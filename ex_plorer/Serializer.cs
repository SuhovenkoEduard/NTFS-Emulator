using ex_plorer.NTFS;
using ex_plorer.NTFS.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ex_plorer
{
    class Serializer
    {
        public string Path;
        public MasterFileTable MFT;
        public XmlSerializer serializer;

        public Serializer(MasterFileTable MFT, string path)
        {
            this.Path = path;
            this.MFT = MFT;
            this.serializer = new XmlSerializer(typeof(List<SerializationFile>));
        }

        ~Serializer() => Serialize();

        public void Serialize()
        {
            
            try
            {
                using (var fs = new System.IO.FileStream(Path, System.IO.FileMode.OpenOrCreate))
                    serializer.Serialize(fs, MFT.files
                        .Select(file =>
                        {
                            SerializationFile serializationFile = new SerializationFile();
                            serializationFile.FileName = file.GetFileName();
                            serializationFile.FileExtension = file.GetFileExtension();
                            serializationFile.FilePath = file.GetFilePath();
                            serializationFile.LastModify = file.GetLastModify();
                            serializationFile.Stream = file.GetStream();
                            serializationFile.IsDirectory = file is Directory;
                            return serializationFile;
                        })
                        .ToList());
            }
            catch (Exception exception)
            {
                MessageBox.Show("Сериализация не удалась. " + exception.Message);
            }
        } 
        public List<IFile> Deserialize()
        {
            List<IFile> result = new List<IFile>();
            try
            {
                using (var fs = new System.IO.FileStream(Path, System.IO.FileMode.Open))
                {
                    List<SerializationFile> serializationFiles = (List<SerializationFile>)serializer.Deserialize(fs);
                    result = serializationFiles.Select(file =>
                    {
                        IFile fi;
                        if (file.IsDirectory)
                            fi = new Directory(MFT, file.FileName, null);
                        else
                            fi = new File(MFT, file.FileName, file.FileExtension, null);
                        return fi;
                    })
                    .ToList();
                }
            } catch (Exception exception)
            {
                MessageBox.Show("Десериализация не удалась. " + exception.Message);
            }

            return result;
        }
    }
}

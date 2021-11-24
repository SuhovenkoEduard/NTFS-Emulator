using ex_plorer.NTFS;
using ex_plorer.NTFS.Files;
using System;
using System.Xml;
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
                using (var fs = new System.IO.FileStream(Path, System.IO.FileMode.Create))
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
                            serializationFile.ParentPath = (file.GetParent() == null ? "" : file.GetParent().GetFilePath());
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
                        if (file.IsDirectory)
                        {
                            Directory restoredDirectory = new Directory(MFT, file.FileName, null);
                            restoredDirectory.FileName = file.FileName;
                            restoredDirectory.FileExtension = file.FileExtension;
                            restoredDirectory.FilePath = file.FilePath;
                            restoredDirectory.LastModify = file.LastModify;
                            return (IFile) restoredDirectory;
                        }
                        else
                        {
                            File restoredFile = new File(MFT, file.FileName, file.FileExtension, null);
                            restoredFile.FileName = file.FileName;
                            restoredFile.FileExtension = file.FileExtension;
                            restoredFile.FilePath = file.FilePath;
                            restoredFile.LastModify = file.LastModify;
                            restoredFile.stream = file.Stream;
                            return (IFile) restoredFile;
                        }
                    }).ToList();

                    for (int i = 0; i < serializationFiles.Count; ++i)
                    {
                        int parentIndex = serializationFiles
                            .FindIndex(sf => sf.FilePath == serializationFiles[i].ParentPath);
                        IFile parent = (parentIndex == -1? 
                            null : result[parentIndex]);
                        result[i].SetParent(parent);
                    }

                    result.Where(iFile => iFile is Directory dir)
                        .ToList()
                        .ForEach(dir =>
                        {
                            dir.SetChilds(result
                                .Where(fff => fff.GetParent() == dir));
                        });
                }
            } catch (Exception exception)
            {
                MessageBox.Show("Десериализация не удалась. " + exception.Message);
            }

            return result;
        }
    }
}

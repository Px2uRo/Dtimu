using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace Dtimu.Core
{
    internal class TagInstance : Tag
    {
        public override TagTypes TagTypes => throw new NotImplementedException();

        public override void Clear()
        {
            throw new NotImplementedException();
        }
    }
    internal class FileInstance : TagLib.File
    {
        string _p;
        public FileInstance(string path) : base(path)
        {
            _p = path;   
        }
        Tag _tag = new TagInstance();
        public override Tag Tag
        {
            get
            {
                _tag.Title = Path.GetFileNameWithoutExtension(_p);
                return _tag;
            }
        }

        public override Properties Properties => new Properties();

        public override Tag GetTag(TagTypes type, bool create) => Tag;

        public override void RemoveTags(TagTypes types)
        {
            throw new NotImplementedException();
        }

        public override void Save()
        {
            throw new NotImplementedException();
        }
    }
}

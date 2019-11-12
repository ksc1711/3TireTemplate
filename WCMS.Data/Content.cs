using System;
using System.Collections.Generic;

//using Global.FrameWork.Data.Mapper.Dapper;

namespace WCMS.Data
{
    public class Content
    {
        public int Seq { get; set; }
        public string FolderName { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Registrant { get; set; }
        public DateTime RegistrantDate { get; set; }
        public Int64 Size { get; set; }
        public string Extension { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public float VideoTime { get; set; }
        public string Continent { get; set; }
        public string Area { get; set; }
        public string Nation { get; set; }
        public string DetailArea { get; set; }
        public string City { get; set; }
        public string Category { get; set; }
        public string PictureDivision { get; set; }
        public string Season { get; set; }
        public string Time { get; set; }
        public DateTime ShootingDay { get; set; }
        public string Quality { get; set; }
        public string Tag { get; set; }
    }

}
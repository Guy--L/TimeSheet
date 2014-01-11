using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPoco;

namespace TimeSheet.Models
{
    public class Levels : UserBase 
    {
        public List<Level> list { get; set; }
    }
    
    public class LevelView : UserBase
    {
        public Level lv { get; set; }
        public LevelView()
        { }

        public LevelView(int id)
        {
            using (tsDB db = new tsDB())
            {
                lv = id == 0 ? (new Level() { LevelId = 0 }) :
                    db.SingleOrDefault<Level>("where LevelId = @0", id);
            }
        }
    }

    public partial class Level
    {
        [ResultColumn] public int UseCount { get; set; }

        public static string all = @"
            select i.levelid
                , i.level
                , i.regularrate
                , i.overtimerate
                , (select count(levelid) from worker where levelid = i.levelid) usecount
                from level i 
            order by i.level
        ";
    }
}

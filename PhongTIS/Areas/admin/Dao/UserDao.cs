using PagedList;
using PhongTIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhongTIS.Areas.admin.Dao
{
    public class UserDao
    {
        PhongTISDB db = null;
        public UserDao() 
        {
            db = new PhongTISDB();
        }
        public IEnumerable<Menu> ListAll(int page, int pageSize)
        { return db.Menus.OrderByDescending(x => x.order).ToPagedList(page, pageSize); }

        public IEnumerable<Exercise> ListAllExercise(int page, int pageSize)
        { return db.Exercises.OrderByDescending(x => x.order).ToPagedList(page, pageSize); }

        public IEnumerable<ScoreStudent> ListAllScore(int page, int pageSize)
        { return db.ScoreStudents.OrderByDescending(x => x.order).ToPagedList(page, pageSize); }
    }
}
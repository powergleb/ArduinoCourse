using System;
using System.Collections.Generic;
using System.Text;

namespace ArduinoCourse.Entities.Users
{
    public class UserList
    {  
        public List<User> Users { get; private set; }

        public UserList()
        {
            Users = new List<User>();
        }

        public User GetUserById(long id)
        {
            return Users.Find(x => x.Id == id);
        }

        public User CreateUser(long id)
        {
            User user = new User()
            {
                Id = id,
                ActualLesson = 0,
                ActualLessonActualTheory = 0,
                ActualLessonActualTest = -1,
                CurrentLessonActualTheory = -1,
                CurrentLessonActualTest = -1,
            };

            Users.Add(user);

            return user;
        }
    }
}

using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace ToDoList.Models
{
    public class Task
    {
        private string _description;
        private int _id;
        private bool _completed;
        private DateTime _dueDate;

        private static string _sortType = "oldToNew";

        public Task(string description, bool completed = false, DateTime dueDate = default(DateTime), int id = 0)
        {
            _description = description;
            _completed = completed;
            _dueDate = dueDate;
            _id = id;
        }

        public override bool Equals(System.Object otherTask)
        {
          if (!(otherTask is Task))
          {
            return false;
          }
          else
          {
             Task newTask = (Task) otherTask;
             bool idEquality = this.GetId() == newTask.GetId();
             bool descriptionEquality = this.GetDescription() == newTask.GetDescription();
             bool completedEquality = this.GetCompleted() == newTask.GetCompleted();
             bool dueDateEquality = this.GetDueDate() == newTask.GetDueDate();
             return (idEquality && descriptionEquality && dueDateEquality && completedEquality);
           }
        }
        public override int GetHashCode()
        {
             return this.GetDescription().GetHashCode();
        }

        public string GetDescription()
        {
            return _description;
        }
        public bool GetCompleted()
        {
          return _completed;
        }
        public DateTime GetDueDate()
        {
          return _dueDate;
        }
        public int GetId()
        {
            return _id;
        }

        public void Save()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();

            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"INSERT INTO tasks (description,completed,dueDate) VALUES (@description, @completed,@dueDate);";

            MySqlParameter description = new MySqlParameter();
            description.ParameterName = "@description";
            description.Value = this._description;
            cmd.Parameters.Add(description);

            MySqlParameter completed = new MySqlParameter();
            completed.ParameterName = "@completed";
            completed.Value = this._completed;
            cmd.Parameters.Add(completed);

            MySqlParameter dueDate = new MySqlParameter();
            dueDate.ParameterName = "@dueDate";
            dueDate.Value = this._dueDate;
            cmd.Parameters.Add(dueDate);

            cmd.ExecuteNonQuery();
            _id = (int) cmd.LastInsertedId;
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }

        public static List<Task> GetAll(string sortType)
        {
            List<Task> allTasks = new List<Task> {};
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            if (sortType == "DueDate")
            {
                cmd.CommandText = @"SELECT * FROM tasks ORDER BY dueDate ASC;";
            }
            else if(sortType == "Description")
            {
                cmd.CommandText = @"SELECT * FROM tasks ORDER BY description ASC;";
            }

            var rdr = cmd.ExecuteReader() as MySqlDataReader;
            while(rdr.Read())
            {
              int taskId = rdr.GetInt32(0);
              string taskDescription = rdr.GetString(1);
              bool taskCompleted = rdr.GetBoolean(2);
              DateTime taskdueDate = rdr.GetDateTime(3);
              Task newTask = new Task(taskDescription, taskCompleted,taskdueDate,taskId);
              allTasks.Add(newTask);
            }
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
            return allTasks;
        }

        public static List<Task> GetCompletedTasks()
        {
          List<Task> completedTasks = new List<Task> {};
          MySqlConnection conn = DB.Connection();
          conn.Open();

          var cmd = conn.CreateCommand() as MySqlCommand;
          cmd.CommandText = @"SELECT * FROM tasks WHERE completed = true;";
          var rdr = cmd.ExecuteReader() as MySqlDataReader;
          while(rdr.Read())
          {
            int taskId = rdr.GetInt32(0);
            string taskDescription = rdr.GetString(1);
            bool taskCompleted = rdr.GetBoolean(2);
            DateTime taskdueDate = rdr.GetDateTime(3);
            Task newTask = new Task(taskDescription, taskCompleted,taskdueDate,taskId);
            completedTasks.Add(newTask);
          }
          conn.Close();
          if (conn != null)
          {
              conn.Dispose();
          }
          return completedTasks;

        }
        public static Task Find(int id)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM tasks WHERE id = (@searchId);";

            MySqlParameter searchId = new MySqlParameter();
            searchId.ParameterName = "@searchId";
            searchId.Value = id;
            cmd.Parameters.Add(searchId);

            var rdr = cmd.ExecuteReader() as MySqlDataReader;
            int taskId = 0;
            string taskName = "";
            DateTime dueDate = DateTime.Now;
            bool taskCompleted = false;

            while(rdr.Read())
            {
              taskId = rdr.GetInt32(0);
              taskName = rdr.GetString(1);
              taskCompleted = rdr.GetBoolean(2);
              dueDate = rdr.GetDateTime(3);
            }
            Task newTask = new Task(taskName, taskCompleted,dueDate, taskId);
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }

            return newTask;
        }

        public void UpdateDescription(string newDescription, bool newCompleted, DateTime newdueDate)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"UPDATE tasks SET description = @newDescription, completed = @newCompleted, dueDate = @newdueDate WHERE id = @searchId;";

            MySqlParameter searchId = new MySqlParameter();
            searchId.ParameterName = "@searchId";
            searchId.Value = _id;
            cmd.Parameters.Add(searchId);

            MySqlParameter description = new MySqlParameter();
            description.ParameterName = "@newDescription";
            description.Value = newDescription;
            cmd.Parameters.Add(description);

            MySqlParameter completed = new MySqlParameter();
            completed.ParameterName = "@newCompleted";
            completed.Value = newCompleted;
            cmd.Parameters.Add(completed);

            MySqlParameter dueDate = new MySqlParameter();
            dueDate.ParameterName = "@newdueDate";
            dueDate.Value = newdueDate;
            cmd.Parameters.Add(dueDate);

            cmd.ExecuteNonQuery();
            _description = newDescription;
            _completed = newCompleted;
            _dueDate = newdueDate;
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }
    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      MySqlCommand cmd = new MySqlCommand("DELETE FROM tasks WHERE id = @TaskId; DELETE FROM categories_tasks WHERE task_id = @TaskId;", conn);
      MySqlParameter taskIdParameter = new MySqlParameter();
      taskIdParameter.ParameterName = "@TaskId";
      taskIdParameter.Value = this.GetId();

      cmd.Parameters.Add(taskIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }
        public static void DeleteAll()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"DELETE FROM tasks;";
            cmd.ExecuteNonQuery();
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }
        public void AddCategory(Category newCategory)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"INSERT INTO categories_tasks (category_id, task_id) VALUES (@CategoryId, @TaskId);";

            MySqlParameter category_id = new MySqlParameter();
            category_id.ParameterName = "@CategoryId";
            category_id.Value = newCategory.GetId();
            cmd.Parameters.Add(category_id);

            MySqlParameter task_id = new MySqlParameter();
            task_id.ParameterName = "@TaskId";
            task_id.Value = _id;
            cmd.Parameters.Add(task_id);

            cmd.ExecuteNonQuery();
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }
        public List<Category> GetCategories()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT category_id FROM categories_tasks WHERE task_id = @taskId;";

            MySqlParameter taskIdParameter = new MySqlParameter();
            taskIdParameter.ParameterName = "@taskId";
            taskIdParameter.Value = _id;
            cmd.Parameters.Add(taskIdParameter);

            var rdr = cmd.ExecuteReader() as MySqlDataReader;

            List<int> categoryIds = new List<int> {};
            while(rdr.Read())
            {
                int categoryId = rdr.GetInt32(0);
                categoryIds.Add(categoryId);
            }
            rdr.Dispose();

            List<Category> categories = new List<Category> {};
            foreach (int categoryId in categoryIds)
            {
                var categoryQuery = conn.CreateCommand() as MySqlCommand;
                categoryQuery.CommandText = @"SELECT * FROM categories WHERE id = @CategoryId;";

                MySqlParameter categoryIdParameter = new MySqlParameter();
                categoryIdParameter.ParameterName = "@CategoryId";
                categoryIdParameter.Value = categoryId;
                categoryQuery.Parameters.Add(categoryIdParameter);

                var categoryQueryRdr = categoryQuery.ExecuteReader() as MySqlDataReader;
                while(categoryQueryRdr.Read())
                {
                    int thisCategoryId = categoryQueryRdr.GetInt32(0);
                    string categoryName = categoryQueryRdr.GetString(1);
                    Category foundCategory = new Category(categoryName, thisCategoryId);
                    categories.Add(foundCategory);
                }
                categoryQueryRdr.Dispose();
            }
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
            return categories;
        }

    }
}

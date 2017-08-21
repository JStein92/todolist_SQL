using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using ToDoList.Models;

namespace ToDoList.Tests
{
  [TestClass]
  public class CategoryTest : IDisposable
  {
    public CategoryTest()
    {
      DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=to_do_test;";
    }
    public void Dispose()
    {
      Task.DeleteAll();
      Category.DeleteAll();
    }

    [TestMethod]
        public void Delete_DeletesCategoryFromDatabase_CategoryList()
        {
          //Arrange
          string name1 = "Home stuff";
          Category testCategory1 = new Category(name1);
          testCategory1.Save();

          string name2 = "Work stuff";
          Category testCategory2 = new Category(name2);
          testCategory2.Save();

          //Act
          testCategory1.Delete();
          List<Category> resultCategories = Category.GetAll();
          List<Category> testCategoryList = new List<Category> {testCategory2};

          //Assert
          CollectionAssert.AreEqual(testCategoryList, resultCategories);
        }
        [TestMethod]
    public void Test_AddTask_AddsTaskToCategory()
    {
      //Arrange
      Category testCategory = new Category("Household chores");
      testCategory.Save();

      Task testTask = new Task("Mow the lawn");
      testTask.Save();

      Task testTask2 = new Task("Water the garden");
      testTask2.Save();

      //Act
      testCategory.AddTask(testTask);
      testCategory.AddTask(testTask2);

      List<Task> result = testCategory.GetTasks();
      List<Task> testList = new List<Task>{testTask, testTask2};

      //Assert
      CollectionAssert.AreEqual(testList, result);
    }
  }
}

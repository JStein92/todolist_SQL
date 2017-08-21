using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;
using System.Collections.Generic;
using System;

namespace ToDoList.Controllers
{
  public class HomeController : Controller
  {
    [HttpGet("/")]
    public ActionResult Index()
    {
      return View();
    }
    [HttpGet("/tasks")]
    public ActionResult Tasks(string buttonSortType)
    {
        string newSortType = buttonSortType;
        Console.WriteLine("THE NEW SORT TYPE IS" + newSortType);

        List<Task> allTasks = Task.GetAll(newSortType);
        return View(allTasks);
    }
    [HttpGet("/categories")]
    public ActionResult Categories()
    {
        List<Category> allCategories = Category.GetAll();
        return View(allCategories);
    }
    [HttpGet("/tasks/new")]
    public ActionResult TaskForm()
    {
        return View();
    }
    [HttpPost("/tasks/new")]
    public ActionResult TaskCreate()
    {
        Task newTask = new Task(Request.Form["task-description"], false, DateTime.Parse(Request.Form["task-dueDate"]));

        newTask.Save();
        return View("Success");
    }
    [HttpGet("/categories/new")]
    public ActionResult CategoryForm()
    {
        return View();
    }
    [HttpPost("/categories/new")]
    public ActionResult CategoryCreate()
    {
        Category newCategory = new Category(Request.Form["category-name"]);
        newCategory.Save();
        return View("Success");
    }
    [HttpGet("/tasks/{id}")]
    public ActionResult TaskDetail(int id)
    {
        Dictionary<string, object> model = new Dictionary<string, object>();
        Task selectedTask = Task.Find(id);
        List<Category> TaskCategories = selectedTask.GetCategories();
        List<Category> AllCategories = Category.GetAll();
        model.Add("task", selectedTask);
        model.Add("taskCategories", TaskCategories);
        model.Add("allCategories", AllCategories);
        return View( model);
    }
    //ONE CATEGORY
    [HttpGet("/categories/{id}")]
    public ActionResult CategoryDetail(int id)
    {
        Dictionary<string, object> model = new Dictionary<string, object>();
        Category SelectedCategory = Category.Find(id);
        List<Task> CategoryTasks = SelectedCategory.GetTasks();
        List<Task> AllTasks = Task.GetAll("oldToNew");
        model.Add("category", SelectedCategory);
        model.Add("categoryTasks", CategoryTasks);
        model.Add("allTasks", AllTasks);
        return View(model);
    }
    //ADD CATEGORY TO TASK
    [HttpPost("task/add_category")]
    public ActionResult TaskAddCategory()
    {
        Category category = Category.Find(Int32.Parse(Request.Form["category-id"]));
        Task task = Task.Find(Int32.Parse(Request.Form["task-id"]));
        task.AddCategory(category);
        return View("Success");
    }
    //ADD TASK TO CATEGORY
    [HttpPost("category/add_task")]
    public ActionResult CategoryAddTask()
    {
        Category category = Category.Find(Int32.Parse(Request.Form["category-id"]));
        Task task = Task.Find(Int32.Parse(Request.Form["task-id"]));
        category.AddTask(task);
        return View("Success");
    }
    [HttpPost("task/update")]
    public ActionResult TaskUpdate()
    {

      Task task = Task.Find(Int32.Parse(Request.Form["task-id"]));
      bool newCompleted = (Request.Form["task-completed"] == "on") ? true : false;
      string newDescription = task.GetDescription();
      DateTime newdueDate = task.GetDueDate();
      task.UpdateDescription(newDescription, newCompleted, newdueDate);

      Console.WriteLine("UPDATE BOOLEAN:" + newCompleted);

      return View("Success");
    }
    [HttpGet("/completedTasks")]
    public ActionResult CompletedTasks()
    {
      List<Task> completedTasks = Task.GetCompletedTasks();
      return View(completedTasks);
    }
  }
}

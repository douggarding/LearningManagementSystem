using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
  [Authorize(Roles = "Student")]
  public class StudentController : CommonController
  {

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Catalog()
    {
      return View();
    }

    public IActionResult Class(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      return View();
    }

    public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      return View();
    }


    public IActionResult ClassListings(string subject, string num)
    {
      System.Diagnostics.Debug.WriteLine(subject + num);
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      return View();
    }


    /*******Begin code to modify********/

    /// <summary>
    /// Returns a JSON array of the classes the given student is enrolled in.
    /// Each object in the array should have the following fields:
    /// "subject" - The subject abbreviation of the class (such as "CS")
    /// "number" - The course number (such as 6016)
    /// "name" - The course name
    /// "season" - The season part of the semester
    /// "year" - The year part of the semester
    /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
    /// </summary>
    /// <param name="uid">The uid of the student</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetMyClasses(string uid)
    {
      var query =
        from E in db.Enrolled
        where E.Student == uid
        join Cl in db.Classes
        on E.Class equals Cl.ClassId into join1
        from j1 in join1
        join Co in db.Courses
        on j1.Offering equals Co.CatalogId
        select new
        {
          subject = Co.Department,
          number = Co.Number,
          name = Co.Name,
          season = j1.Season,
          year = j1.Year,
          grade = E.Grade == null ? "--" : E.Grade
        };

      return Json(query.ToArray());
    }

    /// <summary>
    /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
    /// Each object in the array should have the following fields:
    /// "aname" - The assignment name
    /// "cname" - The category name that the assignment belongs to
    /// "due" - The due Date/Time
    /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="uid"></param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
    {
      var query =
        from S in db.Students
        where S.UId == uid
        join E in db.Enrolled
        on uid equals E.Student into join1
        from j1 in join1
        where j1.Class == num
        join Cl in db.Classes
        on j1.Class equals Cl.ClassId into join2
        from j2 in join2
        where j2.Season == season && j2.Year == year
        join AC in db.AssignmentCategories
        on j2.ClassId equals AC.Class into join3
        from j3 in join3
        join ASG in db.Assignments
        on j3.CategoryId equals ASG.Category into join4
        from j4 in join4
        join SUB in db.Submissions
        on j4.AssignmentId equals SUB.Assignment
        where uid == SUB.Student
        select new { aname = j4.Name, cname = j3.Name, due = j4.Due, score = j4.Points };

      return Json(query.ToArray());
    }


    /// <summary>
    /// Adds a submission to the given assignment for the given student
    /// The submission should use the current time as its DateTime
    /// You can get the current time with DateTime.Now
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The new assignment name</param>
    /// <param name="uid">The student submitting the assignment</param>
    /// <param name="contents">The text contents of the student's submission</param>
    /// <returns>A JSON object containing {success = true/false}</returns>
    public IActionResult SubmitAssignmentText(string subject, int num, string season, int year, string category, string asgname, string uid, string contents)
    {
      bool success = false;
      Submissions new_sub = new Submissions();
      new_sub.Score = 0;
      new_sub.TextContents = contents;
      new_sub.Time = DateTime.Now;
      new_sub.Student = uid;
      var query = 
        from Co in db.Courses
        where Co.Department == subject && Co.Number == num
        join Cl in db.Classes
        on Co.CatalogId equals Cl.Offering into join1
        from j1 in join1
        where j1.Year == year && j1.Season == season
        join AC in db.AssignmentCategories
        on j1.ClassId equals AC.Class into join2
        from j2 in join2
        where j2.Name == category
        join ASG in db.Assignments
        on j2.CategoryId equals ASG.Category into join3
        from j3 in join3
        where j3.Name == asgname
        select j3.AssignmentId;

      if(query.Count() > 1)
      {
        return Json(success);
      }
      new_sub.Assignment = query.First();
      db.Submissions.Add(new_sub);
      try
      {
        db.SaveChanges();
        success = true;
      }
      catch { }

      return Json( success);
    }

    /// <summary>
    /// Enrolls a student in a class.
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester</param>
    /// <param name="year">The year part of the semester</param>
    /// <param name="uid">The uid of the student</param>
    /// <returns>A JSON object containing {success = {true/false}. False if the student is already enrolled in the class.</returns>
    public IActionResult Enroll(string subject, int num, string season, int year, string uid)
    {
      return null;
    }

    /// <summary>
    /// Calculates a student's GPA
    /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
    /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
    /// Otherwise, the point-value of a letter grade for the UofU is determined by the table on this page:
    /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
    /// </summary>
    /// <param name="uid">The uid of the student</param>
    /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
    public IActionResult GetGPA(string uid)
    {
      return null;
    }
  }
}
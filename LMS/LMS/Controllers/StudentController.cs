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
              from S in db.Students // STUDENTS to ENROLLED
              join E in db.Enrolled on S.UId equals E.Student into join1

              from j1 in join1 // ENROLLED to CLASSES
              join Cl in db.Classes on j1.Class equals Cl.ClassId into join2

              from j2 in join2 // CLASSES to COURSES
              join Co in db.Courses on j2.Offering equals Co.CatalogId into join3

              from j3 in join3 // CLASSES to ASSIGNMENT CATEGORIES
              join AC in db.AssignmentCategories on j2.ClassId equals AC.Class into join4

              from j4 in join4 // ASSIGNMENT CATEGORIES to ASSIGNMENTS
              join ASG in db.Assignments on j4.CategoryId equals ASG.Category into join5

              from j5 in join5// SUBMISSIONS to ASSIGNMENTS 
              join SUB in db.Submissions on j5.AssignmentId equals SUB.Assignment into join6

              from j6 in join6.DefaultIfEmpty() // (SUBMISSIONS needs to be left joined to account for null submissions)

              where S.UId == uid // Ensure correct Student
              && j3.Department == subject // Ensure correct subject (e.g.: CS)
              && j3.Number == num // Ensure correct course number (e.g.: 1410)
              && j2.Season == season // Ensure correct season (e.g.: Fall)
              && j2.Year == year // Ensure correct year

              select new {
                  aname = j5.Name,
                  cname = j4.Name,
                  due = j5.Due,
                  score = j6.Score // Okay if null
              };

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

            new_sub.Assignment = query.First();
            db.Submissions.Add(new_sub);
            try
            {
                db.SaveChanges();
                success = true;
            }
            catch { }

            return Json(success);
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
        
            // Check to see if the student is already enrolled
            var query =
              from Co in db.Courses //COURSES to CLASSES
              join Cl in db.Classes on Co.CatalogId equals Cl.Offering into join1
              
              from j1 in join1 // CLASSES to ENROLLED
              join E in db.Enrolled on j1.ClassId equals E.Class into join2

              from j2 in join2
              where Co.Department == subject // Ensure correct course department
              && Co.Number == num // Ensure correct course number
              && j1.Season == season // Ensure correct season
              && j1.Year == year // Ensure class year
              && j2.Student == uid // Ensure student is enrolled 

              select j2.Student;

            // If the student's already enrolled, return a false success
            if (query.Count() == 1)
            {
                var alreadyRegistered = new { success = false };
                return Json(alreadyRegistered);
            }

            // If we're here, then student is not currently enrolled in course
            Enrolled new_enr = new Enrolled();
            new_enr.Student = uid;
            var query2 =
              from Co in db.Courses
              where Co.Number == num && Co.Department == subject
              join Cl in db.Classes
              on Co.CatalogId equals Cl.Offering into join1
              from j1 in join1
              where j1.Season == season && j1.Year == year
              select j1.ClassId;

            new_enr.Class = query2.First();
            db.Enrolled.Add(new_enr);
            try
            {
                db.SaveChanges();
            }
            catch { }

            // Registration complete, so return
            var successfullyRegistered = new { success = true };
            return Json(successfullyRegistered);
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
            var grades =
              from E in db.Enrolled
              where E.Student == uid
              join Cl in db.Classes
              on E.Class equals Cl.ClassId into join1
              from j1 in join1
              join Co in db.Courses
              on j1.Offering equals Co.CatalogId
              select new
              {
                  grade = E.Grade == null ? "--" : E.Grade
              };

            double total = 0;
            int count = 0;
            foreach (var g in grades)
            {
                string grade = g.grade;
                if (grade != "--")
                {
                    count++;
                    switch (grade)
                    {
                        case "A":
                            total += 4;
                            break;
                        case "A-":
                            total += 3.7;
                            break;
                        case "B+":
                            total += 3.3;
                            break;
                        case "B":
                            total += 3;
                            break;
                        case "B-":
                            total += 2.7;
                            break;
                        case "C+":
                            total += 2.3;
                            break;
                        case "C":
                            total += 2;
                            break;
                        case "C-":
                            total += 1.7;
                            break;
                        case "D+":
                            total += 1.3;
                            break;
                        case "D":
                            total += 1;
                            break;
                        case "D-":
                            total += 0.7;
                            break;
                        case "E":
                            break;
                    }
                }
            }
            double GPA = 0.0;
            if (count > 0)
            {
                GPA = total / count;
            }

            // Need to turn GPA into an anonymous type that can be turned into JSON:
            var anonymousGPAobj = new { gpa = GPA };
            return Json(anonymousGPAobj);
        }
    }
}
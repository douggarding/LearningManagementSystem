﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministratorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Department(string subject)
        {
            ViewData["subject"] = subject;
            return View();
        }

        public IActionResult Course(string subject, string num)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }

        /// <summary>
        /// Returns a JSON array of all the courses in the given department.
        /// Each object in the array should have the following fields:
        /// "number" - The course number (as in 6016 for this course)
        /// "name" - The course name (as in "Database Systems..." for this course)
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <returns></returns>
        public IActionResult GetCourses(string subject)
        {
            var query =
                from dep in db.Departments
                join co in db.Courses on dep.DId equals co.Department

                where dep.DId == subject
                select new
                {
                    number = co.Number,
                    name = co.Name
                };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all the professors working in a given department.
        /// Each object in the array should have the following fields:
        /// "lname" - The professor's last name
        /// "fname" - The professor's first name
        /// "uid" - The professor's uid
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <returns></returns>
        public IActionResult GetProfessors(string subject)
        {
            var query =
                from prof in db.Professors
                where prof.WorksIn == subject
                select new
                {
                    lname = prof.LastName,
                    fname = prof.FirstName,
                    uid = prof.UId
                };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Creates a course.
        /// </summary>
        /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
        /// <param name="number">The course number</param>
        /// <param name="name">The course name</param>
        /// <returns>A JSON object containing {success = true/false}. False if the course already exists, true otherwise.</returns>
        public IActionResult CreateCourse(string subject, int number, string name)
        {
            // Return false if the course already exists
            // Course is determined to exist if Department/Number already exist as a combination???
            var doesCourseExistQuery =
                from co in db.Courses
                where co.Number == number
                && co.Department == subject
                select co.CatalogId;

            if(doesCourseExistQuery.Count() > 0)
            {
                return Json(new { success = false});
            }

            // Perform query to find an ideal catalog ID for new course
            var query =
                from co in db.Courses
                select co.CatalogId;

            // Selects the smallest unused catalogID number to use as new catalogID
            int[] queryArray = query.ToArray();
            Array.Sort(queryArray);
            int courseID = queryArray.Length;
            for (int i = 0; i < query.Count(); i++)
            {
                if (queryArray[i] > i)
                {
                    courseID = i;
                    break;
                }
            }

            // Set up the Course
            Models.LMSModels.Courses course = new Models.LMSModels.Courses();
            course.Department = subject;
            course.Number = number;
            course.Name = name;
            course.CatalogId = courseID;

            // Insert the Course into the database
            db.Courses.Add(course);
            try
            {
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch // If inserting changes to database fails
            {
                return Json(new { success = false });
            }

        }

        /// <summary>
        /// Creates a class offering of a given course.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="number">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="start">The start time</param>
        /// <param name="end">The end time</param>
        /// <param name="location">The location</param>
        /// <param name="instructor">The uid of the professor</param>
        /// <returns>A JSON object containing {success = true/false}. False if another class occupies the same location during any time within the start-end range in the same semester.</returns>
        public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
        {
            return null;
        }

    }
}
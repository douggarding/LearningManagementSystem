using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    public class CommonController : Controller
    {

        // Add a protected database context variable once the team deatabase has been scaffolded
        protected Models.LMSModels.Team2Context db;

        public CommonController()
        {
            // Initialize the context once the team database has been scaffolded
            db = new Models.LMSModels.Team2Context();
        }

        /*
         * WARNING: This is the quick and easy way to make the controller
         *          use a different Context - good enough for our purposes.
         *          The "right" way is through Dependency Injection via the constructor (look this up if interested).
        */

        // TODO: Add a "UseContext" method if you wish to change the "db" context for unit testing
        //       See the lecture on testing

        // TODO: Uncomment this once you have created the variable "db"
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Retreive a JSON array of all departments from the database.
        /// Each object in the array should have a field called "name" and "subject",
        /// where "name" is the department name and "subject" is the subject abbreviation.
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetDepartments()
        {
            using (db)
            {
                // Build the query for getting list of departments
                var query =
                from d in db.Departments
                select new
                {
                    name = d.Name, // Department Name
                    subject = d.DId // Subject (Department ID?)
                };

                return Json(query.ToArray());
            }
        }


        /// <summary>
/// Returns a JSON array representing the course catalog.
/// Each object in the array should have the following fields:
/// "subject": The subject abbreviation, (e.g. "CS")
/// "dname": The department name, as in "Computer Science"
/// "courses": An array of JSON objects representing the courses in the department.
///            Each field in this inner-array should have the following fields:
///            "number": The course number (e.g. 6016)
///            "cname": The course name (e.g. "Database Systems and Applications")
/// </summary>
/// <returns>The JSON array</returns>
// Example JSON string:
//[{"subject":"ART","dname":"Art","courses":[{"number":2200,"cname":"Beginning Drawing"},
//{"number":2060,"cname":"Digital Photography"}]},{"subject":"CS","dname":"Computer Science",
//"courses":[{"number":1410,"cname":"Object-Oriented Programming"},{"number":6016,"cname":"Database
 //Systems and Applications"},{"number":2420,"cname":"Introduction to Algorithms and Data Structures"},
 //{"number":3500,"cname":"Software Practice"},{"number":3810,"cname":"Computer Organization"},
 //{"number":5300,"cname":"Artificial Intelligence"}]}
        public IActionResult GetCatalog()
        {
            var query =
                from c in db.Courses
                select new
                {
                    number = c.Number,
                    cname = c.Name
                };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all class offerings of a specific course.
        /// Each object in the array should have the following fields:
        /// "season": the season part of the semester, such as "Fall"
        /// "year": the year part of the semester
        /// "location": the location of the class
        /// "start": the start time in format "hh:mm:ss"
        /// "end": the end time in format "hh:mm:ss"
        /// "fname": the first name of the professor
        /// "lname": the last name of the professor
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public IActionResult GetClassOfferings(string subject, int number)
        {
            var query =
                from classes in db.Classes
                join courses in db.Courses
                on classes.Offering equals courses.CatalogId
                select new
                {
                    season = classes.Season,
                    year = classes.Year,
                    location = classes.Location,
                    start = classes.Start,
                    end = classes.End,
                    fname = classes.TaughtBy
                };

            return Json(query.ToArray());

        }

        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <returns>The assignment contents</returns>
        public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
        {
            return null;
        }

        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment submission.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <param name="uid">The uid of the student who submitted it</param>
        /// <returns>The submission text</returns>
        public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
        {
            return null;
        }


        /// <summary>
        /// Gets information about a user as a single JSON object.
        /// The object should have the following fields:
        /// "fname": the user's first name
        /// "lname": the user's last name
        /// "uid": the user's uid
        /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
        ///               If the user is a Professor, this is the department they work in.
        ///               If the user is a Student, this is the department they major in.    
        ///               If the user is an Administrator, this field is not present in the returned JSON
        /// </summary>
        /// <param name="uid">The ID of the user</param>
        /// <returns>The user JSON object or an object containing {success: false} if the user doesn't exist</returns>
        public IActionResult GetUser(string uid)
        {
            return null;
        }


        /*******End code to modify********/

    }
}
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

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
        /// Example JSON string:
        /// [{"subject":"ART","dname":"Art","courses":[{"number":2200,"cname":"Beginning Drawing"},
        /// {"number":2060,"cname":"Digital Photography"}]},{"subject":"CS","dname":"Computer Science",
        /// "courses":[{"number":1410,"cname":"Object-Oriented Programming"},{"number":6016,"cname":"Database
        /// Systems and Applications"},{"number":2420,"cname":"Introduction to Algorithms and Data Structures"},
        /// {"number":3500,"cname":"Software Practice"},{"number":3810,"cname":"Computer Organization"},
        /// {"number":5300,"cname":"Artificial Intelligence"}]}
        public IActionResult GetCatalog()
        {

            var query =
                from d in db.Departments
                select new
                {
                    subject = d.DId,
                    dname = d.Name,
                    courses = from course in db.Courses
                              where course.Department.Equals(d.DId)
                              select new
                              {
                                  number = course.Number,
                                  cname = course.Name
                              }
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

            // SQL query that got all of the desired values (with the exception of specifying
            // a subject and number):
            //
            // SELECT Season, Year, Location, Start, End, FirstName, LastName FROM
            // Team2.Classes c1 join Team2.Courses co join Team2.Professors p
            // WHERE c1.offering = co.catalogID AND c1.TaughtBy = p.uID
            //
            // Also, as specified in our database, none of the values from these tables
            // can contain null values, so we aren't accounting for them here.
            var query =
                from co in db.Courses
                where co.Department.Contains(subject) & co.Number.Equals(number)
                join cl in db.Classes on co.CatalogId equals cl.Offering into firstJoin
                from fj in firstJoin
                join p in db.Professors on fj.TaughtBy equals p.UId
                select new
                {
                    season = fj.Season,
                    year = fj.Year,
                    location = fj.Location,
                    start = fj.Start,
                    end = fj.End,
                    fname = p.FirstName,
                    lname = p.LastName
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
            // Query to get the the Contents column of the Assignments table
            var query =
              from Co in db.Courses //COURSES to CLASSES
              join Cl in db.Classes on Co.CatalogId equals Cl.Offering into join1

              from j1 in join1 // CLASSES to ASSIGNMENT CATEGORIES
              join assctg in db.AssignmentCategories on j1.ClassId equals assctg.Class into join2

              from j2 in join2 // ASSIGNMENT CATEGORIES to ASSIGNMENT
              join assgn in db.Assignments on j2.CategoryId equals assgn.Category into join3

              from j3 in join3
              where Co.Department == subject // Ensure correct course department
              && Co.Number == num // Ensure correct course number
              && j1.Season == season // Ensure correct season
              && j1.Year == year // Ensure class year
              && j2.Name == category // Ensure correct assignment category
              && j3.Name == asgname // Ensure correct assignment name
              select j3.Contents;

            return Content(query.ToArray().First().ToString());

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

            var query =
                from stu in db.Students // STUDENTS to ENROLLED
                join enr in db.Enrolled on stu.UId equals enr.Student into firstJoin

                from j1 in firstJoin // ENROLLED to CLASSES
                join cla in db.Classes on j1.Class equals cla.ClassId into secondJoin

                from j2 in secondJoin // CLASSES to COURSE
                join co in db.Courses on j2.Offering equals co.CatalogId into join3

                from j3 in join3 // CLASSES to ASSIGNMENT CATEGORIES
                join assgncat in db.AssignmentCategories on j2.ClassId equals assgncat.Class into join4

                from j4 in join4 // ASSIGNMENT CATEGORIES to ASSIGNMENTS
                join assgn in db.Assignments on j4.CategoryId equals assgn.Category into join5

                from j5 in join5 // ASSIGNMENTS to SUBMISSIONS
                join sub in db.Submissions on j5.AssignmentId equals sub.Assignment into join6

                from j6 in join6
                where j2.Season == season
                && j2.Year == year
                && j3.Department == subject
                && j3.Number == num
                && j4.Name == category
                && j5.Name == asgname
                && j6.Student == uid
                select j6.TextContents;

            // Should return an empty string of "" if a submission doesn't exist.
            // NOTE: Our database also allows the content part of a submission to be null
            // The below should account for either a null 'TextContents' or a non-existent entry
            try
            {
                return Content(query.ToArray().First().ToString());
            }
            // If the above fails due to the field being null, function will return ""
            catch (System.InvalidOperationException)
            {
                return Content("");
            }

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
            // SQL query that got all of the desired values:
            //
            // SELECT Uid, FirstName, LastName, dep.Name
            // FROM Team2.Students stu JOIN Team2.Departments dep on stu.Major = dep.dID
            // WHERE stu.uID = "u0000002";


            // Try getting the information if thais uID is for a STUDENT.
            var studentQuery =
                from stu in db.Students
                join dep in db.Departments on stu.Major equals dep.DId
                where stu.UId == uid
                select new
                {
                    fname = stu.FirstName,
                    lname = stu.LastName,
                    uid = stu.UId,
                    department = dep.Name
                };

            // If the uID was in fact for a student, return their info.
            if (studentQuery.Count() == 1)
            {
                return Json(studentQuery.ToArray().First());
            }

            // Try getting the information if this uID is for a PROFESSOR.
            var professorQuery =
                from prof in db.Professors
                where prof.UId.Equals(uid)
                join dep in db.Departments on prof.WorksIn equals dep.DId
                select new
                {
                    fname = prof.FirstName,
                    lname = prof.LastName,
                    uid = prof.UId,
                    department = dep.Name
                };

            // If the uID was in fact for a student, return their info.
            if (professorQuery.Count() == 1)
            {
                return Json(professorQuery.ToArray().First());
            }

            // Try getting the information if this uID is for an ADMINISTRATOR.
            var administratorQuery =
                from admin in db.Administrators
                select new
                {
                    fname = admin.FirstName,
                    lname = admin.LastName,
                    uid = admin.UId,
                };

            // If the uID was in fact for a student, return their info.
            if (administratorQuery.Count() == 1)
            {
                return Json(professorQuery.ToArray().First());
            }

            // If we get this far in the code, that means the user ID doesn't exist
            var noUser = new { success = false };
            return Json(noUser);

        }


        /*******End code to modify********/

    }
}
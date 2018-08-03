using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
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

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
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

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            var query =
                from co in db.Courses // COURSES to CLASSES
                join cl in db.Classes on co.CatalogId equals cl.Offering into join1

                from j1 in join1 // CLASSES to ENROLLED
                join en in db.Enrolled on j1.ClassId equals en.Class into join2

                from j2 in join2 // ENROLLED to STUDNETS
                join st in db.Students on j2.Student equals st.UId

                where co.Department == subject
                && co.Number == num
                && j1.Year == year
                && j1.Season == season

                select new
                {
                    fname = st.FirstName,
                    lname = st.LastName,
                    uid = st.UId,
                    dob = st.DateOfBirth,
                    grade = j2.Grade == null ? "--" : j2.Grade // Not sure what grade to use if null?
                };

            return Json(query.ToArray());
        }



        /// <summary>
        /// Assume that a specific class can not have two categories with the same name.
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            if (category != null) //Use category to get assignments
            {

                var query =
                    from co in db.Courses // COURSES to CLASSES
                    join cl in db.Classes on co.CatalogId equals cl.Offering into join1

                    from j1 in join1 // CLASSES to ASSIGNMENT CATEGORIES
                    join ac in db.AssignmentCategories on j1.ClassId equals ac.Class into join2

                    from j2 in join2 // ASSIGNMENT CATEGORIES to ASSIGNMENTS
                    join agn in db.Assignments on j2.CategoryId equals agn.Category into join3

                    from j3 in join3 // ASSIGNMENTS to SUBMISSIONS

                    where co.Department == subject
                    && co.Number == num
                    && j1.Season == season
                    && j1.Year == year
                    && j2.Name == category
                    select new
                    {
                        aname = j3.Name,
                        cname = j2.Name,
                        due = j3.Due,
                        submissions = (from sub in db.Submissions where sub.Assignment == j3.AssignmentId select sub).Count()
                    };

                return Json(query.ToArray());
            }
            else // get all class assignments.
            {
                var query =
                  from co in db.Courses // COURSES to CLASSES
                  join cl in db.Classes on co.CatalogId equals cl.Offering into join1

                  from j1 in join1 // CLASSES to ASSIGNMENT CATEGORIES
                  join ac in db.AssignmentCategories on j1.ClassId equals ac.Class into join2

                  from j2 in join2 // ASSIGNMENT CATEGORIES to ASSIGNMENTS
                  join agn in db.Assignments on j2.CategoryId equals agn.Category into join3

                  from j3 in join3 // ASSIGNMENTS to SUBMISSIONS

                  where co.Department == subject
                  && co.Number == num
                  && j1.Season == season
                  && j1.Year == year
                  select new
                  {
                      aname = j3.Name,
                      cname = j2.Name,
                      due = j3.Due,
                      submissions = (from sub in db.Submissions where sub.Assignment == j3.AssignmentId select sub).Count()
                  };

                return Json(query.ToArray());
            }
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var query =
                from co in db.Courses // COURSES to CLASSES
                join cl in db.Classes on co.CatalogId equals cl.Offering into join1

                from j1 in join1 // CLASSES to ASSIGNMENT CATEGORIES
                join ac in db.AssignmentCategories on j1.ClassId equals ac.Class

                where co.Department == subject
                && co.Number == num
                && j1.Season == season
                && j1.Year == year
                select new
                {
                    name = ac.Name,
                    weight = ac.Weight
                };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// A class can not have two categories with the same name.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            // Get the class ID to be associated with this assignment category
            var classIDquery =
                from co in db.Courses // Courses to Classes
                join cl in db.Classes on co.CatalogId equals cl.Offering into join1

                // Get Class ID
                from j1 in join1
                where co.Department == subject
                && co.Number == num
                && j1.Season == season
                && j1.Year == year
                select j1.ClassId;

            // Grab all the assignmentCategory IDs
            var categoryIDs =
                from cat in db.AssignmentCategories // Courses to Classes
                select cat.CategoryId;


            // Turn the user ID's into an ordered list of just numbers 
            int[] categoryIDnums = categoryIDs.ToArray();
            Array.Sort(categoryIDnums);

            int newCategoryNumber = 0;
            // If there are no gaps in the uID numbers, use next sequential number
            if (categoryIDnums[categoryIDs.Count() - 1] == categoryIDnums.Count() - 1)
            {
                newCategoryNumber = categoryIDnums.Count();
            }
            // Otherwise find a gap in the numbers for which to create a new ID
            else
            {
                for (int i = 0; i < categoryIDnums.Count(); i++)
                {
                    if (i < categoryIDnums[i]) // Then we found a skipped ID number
                    {
                        newCategoryNumber = i;
                        break;
                    }
                }
            }

           
            // Set up the Category
            Models.LMSModels.AssignmentCategories assignCategory = new Models.LMSModels.AssignmentCategories();
            assignCategory.CategoryId = newCategoryNumber;
            assignCategory.Name = category;
            assignCategory.Weight = catweight;
            assignCategory.Class = classIDquery.ToArray().First();

            // Insert the Category into the database
            db.AssignmentCategories.Add(assignCategory);
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
        /// Creates a new assignment for the given class and category.
        /// An assignment category (which belongs to a class) can not have two assignments with 
        /// the same name.
        /// If an assignment of the given category with the given name already exists, return success = false. 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            // Check if assignment name already exists in the category.
            var query =
                from co in db.Courses // COURSES to CLASSES
                join cl in db.Classes on co.CatalogId equals cl.Offering into join1

                from j1 in join1 // CLASSES to ASSIGNMENT CATEGORIES
                join ac in db.AssignmentCategories on j1.ClassId equals ac.Class into join2

                from j2 in join2 // ASSIGNMENT CATEGORIES to ASSIGNMENTS
                join agn in db.Assignments on j2.CategoryId equals agn.Category into join3
                from j3 in join3

                where co.Department == subject
                && co.Number == num
                && j1.Season == season
                && j1.Year == year
                && j2.Name == category
                && j3.Name == asgname
                select j3.Name;

            // Can't add if the assignment already exists
            if (query.Count() > 0)
            {
                return Json(new { success = false });
            }

            Models.LMSModels.Assignments assignment = new Models.LMSModels.Assignments();
            assignment.Name = asgname;
            assignment.Due = asgdue;
            assignment.Points = asgpoints;
            assignment.Contents = asgcontents;
            assignment.SubmissionType = false;
            // Get category
            var category_query =
                        from co in db.Courses // COURSES to CLASSES
                        join cl in db.Classes on co.CatalogId equals cl.Offering into join1

                        from j1 in join1 // CLASSES to ASSIGNMENT CATEGORIES
                        join ac in db.AssignmentCategories on j1.ClassId equals ac.Class into join2
                        from j2 in join2
                        where co.Department == subject
                        && co.Number == num
                        && j1.Season == season
                        && j1.Year == year
                        && j2.Name == category
                        select j2.CategoryId;
            assignment.Category = category_query.First();

            // Insert the assignment to the database
            db.Assignments.Add(assignment);
            try
            {
                db.SaveChanges();

                var update_query =
                 from co in db.Courses // COURSES to CLASSES
                 join cl in db.Classes on co.CatalogId equals cl.Offering into join1
                 from j1 in join1
                 join E in db.Enrolled on j1.ClassId equals E.Class into join2
                 from j2 in join2
                 where co.Department == subject
                 && co.Number == num
                 && j1.Season == season
                 && j1.Year == year
                 select j2;

                foreach (var j2 in update_query)
                {
                    updateGrade(j2.Student, j2.Class);
                }
                return Json(new { success = true });
            }
            catch // If inserting changes to database fails
            {
                return Json(new { success = false });
            }
        }

        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            var query =
                from co in db.Courses // COURSES to CLASSES
                join cl in db.Classes on co.CatalogId equals cl.Offering into join1

                from j1 in join1 // CLASSES to ASSIGNMENT CATEGORIES
                join ac in db.AssignmentCategories on j1.ClassId equals ac.Class into join2

                from j2 in join2 // ASSIGNMENT CATEGORIES to ASSIGNMENTS
                join agn in db.Assignments on j2.CategoryId equals agn.Category into join3

                from j3 in join3 // ASSIGNMENTS to SUBMISSIONS
                join sb in db.Submissions on j3.AssignmentId equals sb.Assignment into join4

                from j4 in join4 // SUBMISSIONS to STUDENTS
                join st in db.Students on j4.Student equals st.UId into join5
                from j5 in join5

                where co.Department == subject
                && co.Number == num
                && j1.Season == season
                && j1.Year == year
                && j2.Name == category
                && j3.Name == asgname
                select new
                {
                    uid = j4.Student,
                    fname = j5.FirstName,
                    lname = j5.LastName,
                    time = j4.Time,
                    score = j4.Score
                };

            return Json(query.ToArray());
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns> 
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            var query =
              from co in db.Courses // COURSES to CLASSES
              join cl in db.Classes on co.CatalogId equals cl.Offering into join1

              from j1 in join1 // CLASSES to ASSIGNMENT CATEGORIES
              join ac in db.AssignmentCategories on j1.ClassId equals ac.Class into join2

              from j2 in join2 // ASSIGNMENT CATEGORIES to ASSIGNMENTS
              join agn in db.Assignments on j2.CategoryId equals agn.Category into join3

              from j3 in join3 // ASSIGNMENTS to SUBMISSIONS
              join sb in db.Submissions on j3.AssignmentId equals sb.Assignment into join4
              from j4 in join4

              where co.Department == subject
              && co.Number == num
              && j1.Season == season
              && j1.Year == year
              && j2.Name == category
              && j3.Name == asgname
              && j4.Student == uid
              select j4;

            foreach (var j4 in query)
            {
                j4.Score = score;
            }

            try
            {
                db.SaveChanges();

                var update_query =
                  from co in db.Courses // COURSES to CLASSES
                  join cl in db.Classes on co.CatalogId equals cl.Offering into join1
                  from j1 in join1
                  where co.Department == subject
                  && co.Number == num
                  && j1.Season == season
                  && j1.Year == year
                  select j1;

                foreach (var j1 in update_query)
                {
                    updateGrade(uid, j1.ClassId);
                }

                return Json(new { success = true });
            }
            catch // If inserting changes to database fails
            {
                return Json(new { success = false });
            }

        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 6016)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var query =
                from cl in db.Classes // CLASSES to COURSES
                join co in db.Courses on cl.Offering equals co.CatalogId into join1

                from j1 in join1
                where cl.TaughtBy == uid
                select new
                {
                    subject = j1.Department,
                    number = j1.Number,
                    name = j1.Name,
                    season = cl.Season,
                    year = cl.Year
                };

            return Json(query.ToArray());
        }
        /// <summary>
        /// Updates the given student's grade for this class.
        /// </summary>
        /// <param name="uid"> the student's university ID</param>
        /// <param name="classID"> the classes particular instance ID</param>
        public void updateGrade(string uid, int classID)
        {
            // The grade of the class comes from the total over all the categories.
            // Each category has 0 more assignments, from which the points are extracted.
            // Each assignment has a submission by the student (or not), from which the score is obtained.
            // The category weight determines its influence on the overall grade.
            // The grade is then lettered based on the course syllabus. (provided)
            // score comes from submissions, points comes from assignments
            // grade comes from enrolled, weight comes from assignment categories.

            double running_total = 0;
            float tot_weights = 0;

            var get_cats =
              from AC in db.AssignmentCategories
              where AC.Class == classID
              select AC;

            // Check each category for gradeable assignments
            foreach (var AC in get_cats)
            {
                var get_asgns =
                  from agns in db.Assignments
                  where agns.Category == AC.CategoryId
                  select agns;

                var weight = AC.Weight;
                float tot_points = 0;
                float tot_score = 0;

                if (get_asgns.Count() > 0)
                {
                    // This category will be included in determining the grade since it has assignments
                    tot_weights += weight;

                    // Go through all the assignments in this category
                    foreach (var agn in get_asgns)
                    {
                        tot_points += (int)agn.Points;

                        var get_sub =
                          from S in db.Submissions
                          where S.Assignment == agn.AssignmentId
                          && S.Student == uid
                          select new { score = S.Score == null ? 0 : S.Score };

                        if (get_sub.Count() > 0)
                        {
                            tot_score += (int)get_sub.First().score; // error occurs here when there are no submissions.
                        }

                    }

                    running_total += (tot_score / tot_points) * weight;
                }
            }

            // Avoid dividing by a 0. The grade'll be 0 anyways.
            if (tot_weights == 0)
            {
                tot_weights = 1;
            }

            double scale = 100 / tot_weights;
            running_total *= scale;

            string grade = ConvertToLetterGrade(running_total);

            var query =
              from E in db.Enrolled
              where E.Class == classID && E.Student == uid
              select E;

            foreach (var G in query)
            {
                G.Grade = grade;
            }

            try
            {
                db.SaveChanges();
            }
            catch { }
        }

        /// <summary>
        /// Turns a number-value grade into a letter grade.
        /// </summary>
        /// <param name="running_total"> Number value of grade. </param>
        /// <returns></returns>
        private string ConvertToLetterGrade(double running_total)
        {
            string grade = "";

            if (running_total >= 93)
            {
                grade = "A";
            }
            else if (running_total >= 90)
            {
                grade = "A-";
            }
            else if (running_total >= 87)
            {
                grade = "B+";
            }
            else if (running_total >= 83)
            {
                grade = "B";
            }
            else if (running_total >= 80)
            {
                grade = "B-";
            }
            else if (running_total >= 77)
            {
                grade = "C+";
            }
            else if (running_total >= 73)
            {
                grade = "C";
            }
            else if (running_total >= 70)
            {
                grade = "C-";
            }
            else if (running_total >= 67)
            {
                grade = "D+";
            }
            else if (running_total >= 63)
            {
                grade = "D";
            }
            else if (running_total >= 60)
            {
                grade = "D-";
            }
            else
            {
                grade = "E";
            }

            return grade;
        }
    }
}
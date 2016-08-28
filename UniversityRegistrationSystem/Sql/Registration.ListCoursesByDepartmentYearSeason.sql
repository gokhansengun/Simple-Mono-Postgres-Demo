/*
 * Registration.ListCoursesByDepartmentYearSeason.sql
 */

SELECT 
    course_id AS CourseId,
    credit AS Credit,
    department AS Department,
    instructor AS Instructor, 
    season AS Season,
    year AS Year 
FROM courses
WHERE department = @departmentName
AND year = @year
AND season = @season

/*
 * Registration.CoursesByStudent.sql
 */

SELECT 
    c.course_id AS CourseId,
    c.credit AS Credit,
    c.department AS Department,
    c.instructor AS Instructor, 
    season 
FROM taken_courses tc
INNER JOIN courses c ON c.course_id = tc.course_id
WHERE tc.student_id = @studentId

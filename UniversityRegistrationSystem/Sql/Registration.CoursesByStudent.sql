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
INNER JOIN students s ON tc.student_id = s.student_id
INNER JOIN asp_net_users usr ON usr.id = s.user_id
WHERE tc.student_id = @studentId AND usr.id = @userId

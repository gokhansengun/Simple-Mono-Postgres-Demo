CREATE TABLE "public"."courses"( 
	"course_id" varchar(8) NOT NULL,
	"credit" int,
	"department" varchar(8) NOT NULL,
	"instructor" varchar(64) NOT NULL,
    "year" int,
    "season" int
);

CREATE TABLE "public"."students"(
	"student_id" varchar(20) NOT NULL,
	"first_name" varchar(32) NOT NULL,
	"middle_name" varchar(32),
	"last_name" varchar(32) NOT NULL,
	"user_id" varchar(128) NOT NULL
);

CREATE TABLE "public"."taken_courses"(
	"student_id" varchar(20) NOT NULL,
	"course_id" varchar(8) NOT NULL,
	"year" int,
	"season" int
);

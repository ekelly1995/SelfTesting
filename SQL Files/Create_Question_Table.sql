CREATE TABLE dbo.Question(
QuestionID INT IDENTITY(1,1) NOT NULL,
Question NVARCHAR(MAX) NOT NULL,
Answer NVARCHAR(MAX) NULL
);

ALTER TABLE dbo.Question
ADD CONSTRAINT PK_Question_QuestionID PRIMARY KEY CLUSTERED (QuestionID);
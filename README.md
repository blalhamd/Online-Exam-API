# Online Exam API

## Overview

The Online Exam API is a robust backend solution designed to facilitate the creation, management, and execution of online examinations. It provides a comprehensive set of endpoints for handling users (students and teachers), subjects, exams, questions, and exam attempts. Built with a focus on scalability and maintainability, this API serves as the core for any online assessment platform, enabling secure authentication, efficient data management, and flexible exam configurations.




## Key Features

*   **User Management**: Secure authentication and authorization for students and teachers.
*   **Subject Management**: Create, view, and manage academic subjects.
*   **Exam Management**: Comprehensive tools for creating, editing, and deleting exams, including:
    *   Defining exam details (subject, total grade, level, duration, type, description).
    *   Assigning students to specific exams.
    *   Adding and managing multiple-choice questions (MCQs) with choices.
*   **Exam Attempt Tracking**: Record and manage student attempts on exams, including their answers and scores.
*   **Role-Based Access Control**: Granular permissions for different user roles (e.g., ViewStudents, AddStudent, ViewExams, CreateExam).
*   **Pagination**: Efficient retrieval of large datasets for students and exams.
*   **Data Validation**: Robust validation for incoming data to ensure data integrity.




## Entity Relationships

The Online Exam API is built around a well-defined set of entities that model the core components of an online examination system. The relationships between these entities ensure data consistency and enable complex queries and operations.

*   **AppUser**: Represents a user in the system, serving as the base for both `Student` and `Teacher` entities. It holds common authentication and authorization information.
*   **Student**: Extends `AppUser` and represents a student. A student can participate in multiple exams.
*   **Teacher**: Extends `AppUser` and represents a teacher. A teacher can be associated with multiple subjects.
*   **Subject**: Represents an academic subject (e.g., Mathematics, Physics). Exams are created for specific subjects.
*   **Exam**: Represents an online examination. An exam is associated with a `Subject` and can have multiple `ChooseQuestion`s. Students are assigned to exams through the `StudentExam` many-to-many relationship.
*   **ChooseQuestion**: Represents a multiple-choice question. Each `ChooseQuestion` belongs to an `Exam` and has multiple `Choice`s.
*   **Choice**: Represents an option for a `ChooseQuestion`. One of the choices is marked as the correct answer.
*   **StudentExam**: A many-to-many join entity linking `Student` and `Exam`, indicating which students are assigned to which exams.
*   **ExamAttempt**: Records a student's attempt at an exam, including the score and the `UserAnswer`s.
*   **UserAnswer**: Records a student's answer to a specific `ChooseQuestion` within an `ExamAttempt`.




## Technologies Used

The Online Exam API is built using modern and robust technologies to ensure high performance, scalability, and maintainability.

*   **Backend**: ASP.NET Core 8.0
*   **Language**: C#
*   **Database**: Entity Framework Core (Code-First approach) with SQL Server
*   **Authentication**: JWT (JSON Web Tokens) Bearer Authentication
*   **Dependency Injection**: Built-in ASP.NET Core DI container
*   **Validation**: FluentValidation
*   **Logging**: (Implicitly, ASP.NET Core's built-in logging)




## Technical Highlights

*   **Clean Architecture**: The project adheres to principles of clean architecture, separating concerns into `Domain`, `Core`, `Infrastructure`, `Business`, and `API` layers. This promotes maintainability, testability, and scalability.
*   **Repository Pattern**: Implementation of generic and non-generic repositories ensures a clear separation between the business logic and data access layers, making the application more flexible and easier to test.
*   **Unit of Work Pattern**: The Unit of Work pattern is employed to manage transactions and ensure data consistency across multiple operations, providing a single point of entry for database interactions.
*   **Service Layer**: A dedicated service layer (`IServices`) encapsulates business logic, orchestrating operations across various repositories and ensuring that business rules are consistently applied.
*   **JWT Authentication**: Secure authentication is implemented using JSON Web Tokens, providing a stateless and scalable way to handle user sessions and authorize API requests.
*   **FluentValidation**: Input validation is handled using FluentValidation, offering a fluent interface for defining strong-typed validation rules, which enhances data integrity and developer experience.
*   **Migrations**: Entity Framework Core migrations are used for database schema management, allowing for seamless evolution of the database alongside code changes.




## Key Use Cases

*   **Teacher creates a new exam**: A teacher can define a new exam, specify its subject, total grade, duration, and type, and then add multiple-choice questions to it.
*   **Teacher assigns students to an exam**: Teachers can assign a group of students to a specific exam, making it available for them to attempt.
*   **Student takes an exam**: Students can log in, view their assigned exams, and submit their answers to the questions.
*   **System scores an exam**: After a student submits an exam, the system automatically calculates their score based on the correct answers.
*   **Admin manages users**: An administrator can add new students or teachers, view existing user details, and manage their permissions.
*   **Retrieve exam results**: Teachers or administrators can retrieve the results of a specific exam, including student scores and answered questions.




## API Documentation

This section provides detailed documentation for each API endpoint, including HTTP methods, request parameters, and response structures.




### Authentication Endpoints

#### `POST /api/Authentication/login`

Authenticates a user and returns a JWT token.

*   **Request Body (`LoginRequest`)**:

    ```json
    {
        "username": "string",
        "password": "string"
    }
    ```

*   **Response Body (`LoginViewModel`)**:

    ```json
    {
        "token": "string",
        "username": "string",
        "roles": [
            "string"
        ]
    }
    ```




### Students Endpoints

#### `GET /api/Students`

Retrieves a paginated list of students. Requires `Users.ViewStudents` permission.

*   **Query Parameters**:
    *   `pageNumber` (int, optional): The page number to retrieve. Default is 1.
    *   `pageSize` (int, optional): The number of items per page. Default is 10.

*   **Response Body (`PaginatedResponse<StudentViewModel>`)**:

    ```json
    {
        "pageNumber": 1,
        "pageSize": 10,
        "totalCount": 100,
        "items": [
            {
                "id": 1,
                "userId": "guid",
                "userName": "string",
                "email": "string"
            }
        ]
    }
    ```

#### `POST /api/Students`

Creates a new student. Requires `Users.AddStudent` permission.

*   **Request Body (`CreateStudentDto`)**:

    ```json
    {
        "userName": "string",
        "email": "string",
        "password": "string"
    }
    ```

*   **Response**: No content (200 OK on success).




### Exams Endpoints

#### `GET /api/Exams`

Retrieves a paginated list of exams. Requires `Exams.View` permission.

*   **Query Parameters**:
    *   `pageNumber` (int, optional): The page number to retrieve. Default is 1.
    *   `pageSize` (int, optional): The number of items per page. Default is 10.

*   **Response Body (`PaginatedResponse<ExamViewModel>`)**:

    ```json
    {
        "pageNumber": 1,
        "pageSize": 10,
        "totalCount": 50,
        "items": [
            {
                "id": 1,
                "subjectId": 1,
                "subjectName": "Math",
                "totalGrade": 100,
                "level": 1,
                "duration": "01:00:00",
                "examType": 0, // 0 for MCQ, 1 for TrueFalse, etc.
                "description": "Midterm Exam",
                "status": true,
                "numberOfQuestions": 20
            }
        ]
    }
    ```

#### `GET /api/Exams/{examId}`

Retrieves a specific exam by ID. Requires `Exams.ViewById` permission.

*   **Path Parameters**:
    *   `examId` (int, required): The ID of the exam to retrieve.

*   **Response Body (`ExamViewModel`)**:

    ```json
    {
        "id": 1,
        "subjectId": 1,
        "subjectName": "Math",
        "totalGrade": 100,
        "level": 1,
        "duration": "01:00:00",
        "examType": 0,
        "description": "Midterm Exam",
        "status": true,
        "numberOfQuestions": 20
    }
    ```

#### `POST /api/Exams/{examId}/assign-students`

Assigns a list of students to an exam. Requires `Exams.AssignStudents` permission.

*   **Path Parameters**:
    *   `examId` (int, required): The ID of the exam to assign students to.

*   **Request Body (`List<int>`)**:

    ```json
    [
        1, // studentId 1
        2  // studentId 2
    ]
    ```

*   **Response**: No content (200 OK on success).

#### `POST /api/Exams`

Creates a new exam. Requires `Exams.Create` permission.

*   **Request Body (`CreateExamDto`)**:

    ```json
    {
        "subjectId": 1,
        "totalGrade": 100,
        "level": 1,
        "duration": "01:00:00",
        "examType": 0,
        "description": "Final Exam",
        "status": true
    }
    ```

*   **Response**: No content (200 OK on success).

#### `PUT /api/Exams/{examId}`

Edits an existing exam. Requires `Exams.Edit` permission.

*   **Path Parameters**:
    *   `examId` (int, required): The ID of the exam to edit.

*   **Request Body (`CreateExamDto`)**:

    ```json
    {
        "subjectId": 1,
        "totalGrade": 120,
        "level": 2,
        "duration": "01:30:00",
        "examType": 0,
        "description": "Updated Final Exam",
        "status": true
    }
    ```

*   **Response**: No content (200 OK on success).

#### `DELETE /api/Exams/{examId}`

Deletes an exam. Requires `Exams.Delete` permission.

*   **Path Parameters**:
    *   `examId` (int, required): The ID of the exam to delete.

*   **Response**: No content (200 OK on success).

#### `POST /api/Exams/{examId}/questions/mcq`

Adds a multiple-choice question to an exam. Requires `Exams.CreateChooseQuestion` permission.

*   **Path Parameters**:
    *   `examId` (int, required): The ID of the exam to add the question to.

*   **Request Body (`CreateChooseQuestionDto`)**:

    ```json
    {
        "questionText": "What is 2+2?",
        "grade": 5,
        "choices": [
            {
                "choiceText": "3",
                "isCorrect": false
            },
            {
                "choiceText": "4",
                "isCorrect": true
            }
        ]
    }
    ```

*   **Response**: No content (200 OK on success).

#### `PUT /api/Exams/{examId}/questions/mcq/{questionId}`

Updates a multiple-choice question in an exam. Requires `Exams.EditChooseQuestion` permission.

*   **Path Parameters**:
    *   `examId` (int, required): The ID of the exam containing the question.
    *   `questionId` (int, required): The ID of the question to update.

*   **Request Body (`CreateChooseQuestionDto`)**:

    ```json
    {
        "questionText": "What is 2+3?",
        "grade": 5,
        "choices": [
            {
                "choiceText": "4",
                "isCorrect": false
            },
            {
                "choiceText": "5",
                "isCorrect": true
            }
        ]
    }
    ```

*   **Response**: No content (200 OK on success).

#### `DELETE /api/Exams/{examId}/questions/mcq/{questionId}`

Deletes a multiple-choice question from an exam. Requires `Exams.DeleteChooseQuestion` permission.

*   **Path Parameters**:
    *   `examId` (int, required): The ID of the exam containing the question.
    *   `questionId` (int, required): The ID of the question to delete.

*   **Response**: No content (200 OK on success).




# API Endpoint TODOs

## Progress

- [x] Authentication (`AuthController.cs`)
    - [x] Register `/api/auth/register` | `POST`
    - [x] Login `/api/auth/login` | `POST`

- [x] Project and Member Management (`ProjectsController.cs`)
    - [x] Get all projects `/api/projects` | `GET`
    - [x] Get single project `/api/projects/{id}` | `GET`
    - [x] Create project `/api/projects` | `POST`
    - [ ] Project update `/api/projects/{id}` | `PUT` / `PATCH`
    - [ ] Project delete `/api/projects/{id}` | `DELETE`
    - [ ] Add member `/api/projects/{projectId}/users` | `POST`
    - [ ] Remove member `/api/projects/{projectId}/users/{userId}` | `DELETE`
    - [ ] Update member role `/api/projects/{projectId}/users/{userId}/role` | `PUT` / `PATCH`

- [ ] Task and Tag Management (`TasksController.cs`)
    - [ ] Get all tasks `/api/projects/{projectId}/tasks` | `GET`
    - [ ] Get single task `/api/projects/{projectId}/tasks/{taskId}` | `GET`
    - [ ] Create task `/api/projects/{projectId}/tasks` | `POST`
    - [ ] Update task `/api/projects/{projectId}/tasks/{taskId}` | `PUT` / `PATCH`
    - [ ] Delete task `/api/projects/{projectId}/tasks/{taskId}` | `DELETE`
    - [ ] Add tag `/api/tasks/{taskId}/tags/{tagId}` | `POST`
    - [ ] Remove tag `/api/tasks/{taskId}/tags/{tagId}` | `DELETE`

- [ ] Comment Management (`CommentsController.cs`)
    - [ ] Get task comments `/api/tasks/{taskId}/comments` | `GET`
    - [ ] Post new comment `/api/tasks/{taskId}/comments` | `POST`
    - [ ] Reply to comment `/api/tasks/{taskId}/comments` | `POST`
    - [ ] Update comment `/api/comments/{commentId}` | `PUT` / `PATCH`
    - [ ] Delete comment `/api/comments/{commentId}` | `DELETE`

- [ ] User and Group Management
    - [ ] User Endpoints (`UsersController.cs`)
        - [ ] Get my profile `/api/users/me` | `GET`
        - [ ] Update profile `/api/users/me` | `PUT` / `PATCH`
        - [ ] Get user profile `/api/users/{id}` | `GET`
    - [ ] Group Endpoints (`GroupsController.cs`)
        - [ ] Get all groups `/api/groups` | `GET`
        - [ ] Create group `/api/groups` | `POST`
        - [ ] Get group `/api/groups/{id}` | `GET`
        - [ ] Update group `/api/groups/{id}` | `PUT`
        - [ ] Delete group `/api/groups/{id}` | `DELETE`
        - [ ] Add member `/api/groups/{groupId}/users` | `POST`
        - [ ] Remove member `/api/groups/{groupId}/users/{userId}` | `DELETE`

---

### 1. Authentication (`AuthController.cs`)

Implements user registration and login using JWT authentication.

| Status | Endpoint | HTTP Method | Implementation Goal |
| :---: | :--- | :--- | :--- |
| ✅ | `/api/auth/register` | `POST` | Register a new user with hashed password and default role `Worker`. |
| ✅ | `/api/auth/login` | `POST` | Validate credentials and return a JWT containing user role and ID claims. |

---

### 2. Project and Member Management (`ProjectsController.cs`)

Complete the core CRUD functionality for projects and add member management endpoints.

| Status | To-Do | Endpoint | HTTP Method | Implementation Goal |
| :---: | :--- | :--- | :--- | :--- |
| ✅ | **Get All Projects** | `/api/projects` | `GET` | Retrieve projects owned by the logged-in user. |
| ✅ | **Get Single Project** | `/api/projects/{id}` | `GET` | Retrieve project details if owned by the authenticated user. |
| ✅ | **Create Project** | `/api/projects` | `POST` | Create a new project with the logged-in user as owner. |
| ☐ | **Project Update** | `/api/projects/{id}` | `PUT`/`PATCH` | Allow owner/manager to update project details like **name, description, due date, status, priority, and visibility**. |
| ☐ | **Project Delete** | `/api/projects/{id}` | `DELETE` | Permanently remove a project and all associated tasks, comments, and permissions. |
| ☐ | **Add Member** | `/api/projects/{projectId}/users` | `POST` | Add a user to a project (`UserProject` + `ProjectPermission` rows). |
| ☐ | **Remove Member** | `/api/projects/{projectId}/users/{userId}` | `DELETE` | Remove a user from a project. |
| ☐ | **Update Member Role** | `/api/projects/{projectId}/users/{userId}/role` | `PUT`/`PATCH` | Update a user's project-specific role in `ProjectPermission`. |

---

### 3. Task and Tag Management (`TasksController.cs`)

*Controller not yet created.*

Implement full CRUD for tasks, nested under projects, and add endpoints to handle task tagging.

| Status | To-Do | Endpoint | HTTP Method | Implementation Goal |
| :---: | :--- | :--- | :--- | :--- |
| ☐ | **Get All Tasks** | `/api/projects/{projectId}/tasks` | `GET` | List all tasks for a specific project. |
| ☐ | **Get Single Task** | `/api/projects/{projectId}/tasks/{taskId}` | `GET` | Retrieve task details including **assigned user, status, and priority**. |
| ☐ | **Create Task** | `/api/projects/{projectId}/tasks` | `POST` | Create a new task and associate it with the project. |
| ☐ | **Update Task** | `/api/projects/{projectId}/tasks/{taskId}` | `PUT`/`PATCH` | Update task details such as **title, description, status, priority, due date, assigned user**. |
| ☐ | **Delete Task** | `/api/projects/{projectId}/tasks/{taskId}` | `DELETE` | Remove a task. |
| ☐ | **Add Tag** | `/api/tasks/{taskId}/tags/{tagId}` | `POST` | Create a link between a task and a tag (`TaskTag` join table). |
| ☐ | **Remove Tag** | `/api/tasks/{taskId}/tags/{tagId}` | `DELETE` | Delete a task–tag relationship. |

---

### 4. Comment Management (`CommentsController.cs`)

*Controller not yet created.*

Add endpoints for posting, reading, and managing comments and their replies on tasks.

| Status | To-Do | Endpoint | HTTP Method | Implementation Goal |
| :---: | :--- | :--- | :--- | :--- |
| ☐ | **Get Task Comments** | `/api/tasks/{taskId}/comments` | `GET` | Retrieve all top-level comments and their replies for a task, ordered by `CreatedAt`. |
| ☐ | **Post New Comment** | `/api/tasks/{taskId}/comments` | `POST` | Create a new top-level comment. |
| ☐ | **Reply to Comment** | `/api/tasks/{taskId}/comments` | `POST` | Create a new comment with a `ParentCommentId` for threading. |
| ☐ | **Update Comment** | `/api/comments/{commentId}` | `PUT`/`PATCH` | Allow the user to edit their own comment content. |
| ☐ | **Delete Comment** | `/api/comments/{commentId}` | `DELETE` | Remove a comment. |

---

### 5. User and Group Management

*Controllers not yet created.*

Create dedicated controllers for managing user profiles and groups.

#### A. User Endpoints (`UsersController.cs`)

| Status | To-Do | Endpoint | HTTP Method | Implementation Goal |
| :---: | :--- | :--- | :--- | :--- |
| ☐ | **Get My Profile** | `/api/users/me` | `GET` | Retrieve the currently authenticated user's details. |
| ☐ | **Update Profile** | `/api/users/me` | `PUT`/`PATCH` | Update fields like **FirstName, LastName, Avatar**, or change password. |
| ☐ | **Get User Profile** | `/api/users/{id}` | `GET` | Retrieve public details of a specific user. |

#### B. Group Endpoints (`GroupsController.cs`)

| Status | To-Do | Endpoint | HTTP Method | Implementation Goal |
| :---: | :--- | :--- | :--- | :--- |
| ☐ | **Get All Groups** | `/api/groups` | `GET` | List all groups owned by or belonging to the current user. |
| ☐ | **CRUD Group** | `/api/groups` (POST), `/api/groups/{id}` (GET, PUT, DELETE) | `POST`, `GET`, `PUT`, `DELETE` | Full CRUD for the **Group** entity. |
| ☐ | **Add Member** | `/api/groups/{groupId}/users` | `POST` | Add a user to a group (`UserGroup` join table). |
| ☐ | **Remove Member** | `/api/groups/{groupId}/users/{userId}` | `DELETE` | Remove a user from a group. |

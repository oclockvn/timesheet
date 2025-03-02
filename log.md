# Project Log

## 2024-03-02

### Project Exploration
- Explored the project structure to understand the codebase organization
- Generated a tree structure of the codebase to visualize the project hierarchy
- Identified the main components: Core, CLI, and Test projects

### Project Structure
The project follows a standard .NET solution structure with multiple projects:
- TimesheetCli.Core: Contains the business logic, database context, and entity models
- TimesheetCli.Cli: Contains the command-line interface implementation
- TimesheetCli.Test: Contains unit tests for the application

The Core project is further organized into:
- Db: Contains database-related code including the ApplicationDbContext
- Services: Will contain service implementations for business logic

This structure follows good separation of concerns principles, separating the core business logic from the presentation layer (CLI).

# Development Log

## 2024-03-02: Entity Framework Core Model Design

### What was done
- Created the following entities in the `TimesheetCli.Core.Db.Entity` namespace:
  - `User`: Represents a user account with authentication information
  - `Project`: Represents a project that can contain multiple tasks
  - `Task`: Represents a task that can be associated with a project and time entries
  - `TimeEntry`: Represents a time tracking entry for a specific task

### Why it was done
- To fulfill the requirements of the timesheet CLI application:
  - Allow users to register and create accounts
  - Allow users to add projects
  - Allow users to add tasks
  - Allow users to record time entries for specific tasks

### How it was done
- Created entity classes with appropriate properties and relationships
- Configured the `ApplicationDbContext` to include these entities
- Set up entity configurations in the `OnModelCreating` method:
  - Defined primary keys
  - Set required fields and maximum lengths
  - Configured relationships between entities
  - Set up appropriate delete behaviors
  - Added unique indexes for username and email

### Technical decisions
- Made `ProjectId` nullable in the `Task` entity to allow tasks to exist without being associated with a project
- Used `DeleteBehavior.Restrict` for most relationships to prevent cascading deletes that could lead to data loss
- Used `DeleteBehavior.SetNull` for the optional relationship between `Task` and `Project`
- Added a computed `Duration` property to `TimeEntry` to easily calculate the duration of a time entry
- Added timestamps (`CreatedAt` and `UpdatedAt`) to all entities for auditing purposes

## 2024-03-02: Added Validation Annotations to Entities

### What was done
- Added data annotation attributes to all entity classes:
  - Added `[Required]` attribute to required fields
  - Added `[StringLength]` attribute to string properties with appropriate maximum lengths
  - Added `[EmailAddress]` attribute to the Email property in the User entity
  - Added minimum length constraints to name fields

### Why it was done
- To provide an additional layer of validation beyond the Fluent API configurations
- To ensure data integrity at the entity level
- To make validation requirements more explicit in the code

### How it was done
- Added the `System.ComponentModel.DataAnnotations` namespace to all entity classes
- Applied appropriate validation attributes to each property:
  - `[Required]` for non-nullable properties
  - `[StringLength(250)]` for string properties as per requirements
  - More specific length constraints for certain fields (e.g., Username, Name)
  - `[EmailAddress]` for the Email property to ensure valid email format

### Technical decisions
- Used both data annotations and Fluent API configurations for validation:
  - Data annotations provide a clear, declarative way to specify validation rules directly on the entity
  - Fluent API configurations in the DbContext provide more control over database schema generation
- Set minimum length constraints on name fields to prevent empty or too short names
- Kept the maximum string length at 250 characters as specified in the requirements, except for fields that already had more restrictive constraints in the DbContext configuration

## 2024-03-02: Updated DateTime Property Names to Use Utc Suffix

### What was done
- Renamed all DateTime properties to use the "Utc" suffix:
  - Changed `CreatedAt` to `CreatedUtc`
  - Changed `UpdatedAt` to `UpdatedUtc`
  - Changed `StartTime` to `StartTimeUtc`
  - Changed `EndTime` to `EndTimeUtc`
- Updated the computed `Duration` property to use the new property names

### Why it was done
- To clearly indicate that all DateTime values are stored in UTC format
- To follow a consistent naming convention for all DateTime properties
- To prevent timezone-related issues when working with dates and times

### How it was done
- Updated all entity classes to use the new property names
- Updated the computed `Duration` property in the `TimeEntry` entity to use the new property names

### Technical decisions
- Used the "Utc" suffix to make it explicit that all DateTime values are stored in UTC format
- Consistently applied the naming convention across all entities
- This approach helps prevent timezone-related bugs and makes it clear to developers that they need to handle timezone conversions when displaying dates and times to users 
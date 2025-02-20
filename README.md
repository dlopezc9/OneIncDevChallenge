The project is done in Onion Architecture:
- It creates the perfect structure for a simple API that separates concerns in an efficient, easy-to-read, and clear way that allows the project to be fast and enables developers to get a quick grasp of it.
- It allows for a decoupled infrastructure, which still allows every component to be changed independently.
- It allows easy testing.
- It is aligned with Domain-Driven Design, as business rules and domain logic remain isolated from external changes.

About Best Practices:
- Separation of Concerns: Focuses heavily on single responsibility in each layer and class.
- Domain-Centric Design: Allows business logic to be independent of the technologies themselves.
- Dependency Inversion: Allows inner layers to be independent.
- Dependency Injection: Enables easy testability and decoupling.
- AAA Testing: Allows easy tracking of changes in the logic.
- Self-Commenting Code: The code is designed to be easy to read, easy to understand, and self-explanatory.
- Encapsulated Validation: Allows validation to be performed at any given time and only when needed.
- Middleware: Allows to check for exceptions without bloating the code.
- Asynchronous Programming with Cancellation Tokens: Enables the app to be faster and more responsive to user changes.
- Design Patterns: Includes patterns such as the Repository Pattern and IDateTime, which improve testability and enable better implementation of different functions.

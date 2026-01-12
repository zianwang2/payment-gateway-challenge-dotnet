# Design and Implementation Notes
## 1. Persistence Layer

I refactored the persistence model. Originally, the system stored `PostPaymentResponse`, but I believe request/response models should not be tightly coupled with the storage schema. To address this, I redesigned the storage model as `PaymentData`.

This approach provides greater flexibility and allows us to persist data that may differ from both the request and response models. For example, we could store the `full card number` (assuming no compliance or regulatory risks) or additional fields such as an `AuthenticationCode`.

Object mapping between models is handled via the `PaymentExtensions` class, which centralizes and standardizes the conversion logic.

## 2. API and Interface Design

I defined interfaces using `IBankServiceClient` and `IPaymentsRepository`. This abstraction ensures that changes in implementation details do not affect upper layers, effectively decoupling the controller from both external services and the persistence layer.

An additional benefit of this design is improved testability: unit tests can easily mock these interfaces, making tests more isolated, reliable, and maintainable.

## 3. Parameter Validation and Serialization

Parameter validation is primarily implemented using attributes. Most fields are validated using regular expressions. For cases that require cross-field validation (such as validating month and year together), I override the Validate method.

For currency validation, considering both extensibility and reuse, I introduced an `AllowedIsoCurrencyAttribute`.

In Program.cs, I customized the API behavior by overriding the default validation failure handling, returning a tailored 400 Bad Request response body.

For request and response mapping, I extensively use JsonConverter to ensure precise control over serialization and deserialization.

## 4. Logging

Logging is implemented via ILogger to capture key requests and state transitions. This enables effective troubleshooting, root cause analysis, and operational observability when issues occur.

## 5. Exception Handling

I defined a custom `ProviderException` to signal failures when the bank service does not return an expected or acceptable response. This exception is propagated to upper layers to ensure proper handling.

If multiple exception types or different categories of ProviderException are required, an `ErrorCode` can be embedded within the exception and used to determine the appropriate HTTP status code. Given the limited number of exception scenarios in this project, I simplified the design by relying on an ErrorMessage instead.

## 6. Testing

I implemented unit tests for the following components: `Controllers`,`BankServiceClient` and `PaymentExtensions`. These components contain the core business and integration logic, making them the most critical areas to validate through unit testing.
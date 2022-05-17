# What was done?

## Infrastructure
* Project was migrated to .Net 6;
* CI GitHub Actions for push action was added;
* _TreatWarningsAsError_ was enabled;
* High-performance logging was added to project ([High-performance logging in .NET | Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging))

## Project
* All code was converted to Controller/Service/Repository structure and was divided by separate assemblies;
* Contracts were moved to separate assembly;
* Domain models were created;
* All models were made immutable;
* All ApplicationException was changed to HTTP status code (except for POST api/v1/bookings, because it is a part of contract);
* All code was made asynchronous or working with Tasks (like DictionaryRepository);
* Transaction-like behaviour in DictionaryRepositories were made via lock statement;
* DateTime in business-logic was changed by DateOnly for convenience of working with dates;

## Tasks
* New property PreparationTimeInDays in _GET /api/v1/rentals/{int}_ was added;
* New property Unit in _GET /api/v1/booking/{int}_ was added;
* New property PreparationTimes in _GET /api/v1/calendar_ was added;
* Endpoint _PUT /api/v1/rentals/{int}_ was created;

## Tests
* VacationRental.Api.Tests was renamed to VacationRental.Api.Tests.Integration;
* Clients for all controllers were made in VacationRental.Api.Tests.Integration;
* Unit tests were created;
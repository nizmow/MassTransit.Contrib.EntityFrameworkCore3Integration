## NOTE

This is now obsolete, as the EF Core 3.1 changes here plus a few other things have been [merged](https://github.com/phatboyg/MassTransit/commit/cd805c95d0d9180b31eabcad2b00428f95c2cd15) into MassTransit 6.1.

## Entity Framework Core 3.1 for MassTransit

This is a port of the MassTransit Entity Framework Core persistence providers to Entity Framework Core 3.1. It is NOT part of the core MassTransit repository, and should probably be considered temporary until support is eventually merged into MassTransit proper.

It's more or less a drop-in replacement for the existing EFCore provider, documentation about which can be found [here](https://masstransit-project.com/usage/sagas/persistence.html#entity-framework). You should use it the same way, with ONE exception, noted below.

### ISSUES

* The EFCore 2.2 `EntityFrameworkSagaRepository` has the capability to provide a custom query which applies on the saga instance, this capability has been removed.
* Support for .NET Framework 4.6.1 has been dropped.

## Building

* Install .NET Core 3.1 or higher.
* Clone this repository.
* Install the required dotnet tools from the manifest with `dotnet tool restore`.
* Build with `dotnet cake --target=Build`.

...or just load the solution into your favourite editor.

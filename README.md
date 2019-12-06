This is a port of the MassTransit Entity Framework Core persistence providers to Entity Framework Core 3.1. It is NOT part of the core MassTransit repository, and should probably be considered temporary until support is eventually merged into MassTransit proper.

It's more or less a drop-in replacement for the existing EFCore provider, documentation about which can be found [here](https://masstransit-project.com/usage/sagas/persistence.html#entity-framework). You should use it the same way, with ONE exception, noted below.

### ISSUES

* The EFCore 2.2 `EntityFrameworkSagaRepository` has the capability to provide a custom query which applies on the saga instance, this capability has been removed.

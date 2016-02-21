# Profiling2 Migrations

This project contains the FluentMigrator migration classes and SQL scripts to setup a new instance of Profiling2's database from scratch.

Database changes should also be versioned by creating a Migration class.

## Setup

- Create a new database in SQL Server Management Studio (SSMS)
- Add a file 'ftrow_Ft_sources' with filegroup 'ftfg_Ft_sources'
- Add a file 'SourceDocuments' with filegroup 'SourceDocuments'
- Create new query for this database and open the file '201301221832_schema.sql' (this will create the schema, adding stored procedures, functions, triggers, and the full text catalog for PRF_Source table)

- Create new login via Security->Login->Create new login...
- Add login to new database via Databases->(database name)->Security->Users->Create new user...
- Check 'db_owner' under 'database role membership'

## Migrating

These instructions utilise the command line Migration Runner detailed in the [FluentMigrator wiki](https://github.com/schambers/fluentmigrator/wiki/Migration-Runners).

Edit the Connection property Build/Migrate.proj with the local database settings for your machine.

To run all migrations:

    shell> cd \path\to\project
    shell> cd Build
    shell> Migrate.cmd

## Production

To run the command line Migration Runner outside your build environment, do the following:

- Copy the following files to a directory on the production server:
    - Build\Migrate.proj
    - Build\Migrate.cmd
    - Packages\FluentMigrator.1.1.1.0\tools\FluentMigrator.MSBuild.dll
    - Solutions\Profiling2.Migrations\bin\Release\Profiling2.Migrations.dll
- Configure the values for the AssemblyFile, Connection and TargetDll properties at the top of Migrate.proj.
- Execute `Migrate.cmd` to migrate to the most recent migration; `Migrate.cmd /t:MigrateRollback` to rollback one migration.
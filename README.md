# VC issuer GUI
A GUI for the [VC issuer](https://github.com/mmlab-aueb/vc-issuer) implemented by the [ZeroTrustVC](https://mm.aueb.gr/projects/zerotrustvc) project.

## Usage

### Prerequisites
Make sure you have installed [VC issuer](https://github.com/mmlab-aueb/vc-issuer)

### Compile and run
You can open the source code in Visual Studio or you can use .net sdk to compile it.
Instructions for compiling and running the project follow. In order to compile
the source code, from the project folder execute:

```bash
dotnet build
```

In order to run the compiled file, from the project folder execute:

```bash
dotnet run
```

If you have used the provided SQL commands for filling the database with
test records, you can test that everything works by requesting a token using
the following `curl` command

**ΝΟΤΕ**

VC issuer should be installed behind a proxy, which will support HTTPS (see
for example the instructions [here](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-apache?view=aspnetcore-5.0))
as well as password restricted access control (e.g., using [Apache2 Authentication and Authorization](https://httpd.apache.org/docs/2.4/howto/auth.html)).
:: === Deployed with version:
::Azure Functions Core Tools
::Core Tools Version:       3.0.3904 Commit hash: c345f7140a8f968c5dbc621f8a8374d8e3234206  (64-bit)
::Function Runtime Version: 3.3.1.0

:: When deploying to dev, you need to be on subscription Aarsleff.Integrations.dev
:: When deploying to test and prod, one needs to change subscription to Aarsleff.Integrations.test (or prod)

func.exe azure functionapp publish func-shared-utilites-sqlite-mapper-dev --dotnet-version 5.0
::func.exe azure functionapp publish func-shared-utilites-sqlite-mapper-test --dotnet-version 5.0
::func.exe azure functionapp publish func-shared-utilites-sqlite-mapper-prod --dotnet-version 5.0


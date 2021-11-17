:: TODO: for other than test

:: test
:: az functionapp create -g rg-shared-utilities-test -p "/subscriptions/ab3f3323-e618-4892-8816-cc8b03a9cf76/resourceGroups/rg-shared-plans-test/providers/Microsoft.Web/serverfarms/plan-shared-windows-01-test" -n func-shared-utilites-sqlite-mapper-test -s stsharedutilitiestest --functions-version 3

:: prod
az functionapp create -g rg-shared-utilities-prod -p "/subscriptions/3c21cc54-e403-4844-8e08-606529edfbf5/resourceGroups/rg-shared-plans-prod/providers/Microsoft.Web/serverfarms/plan-shared-windows-01-prod" -n func-shared-utilites-sqlite-mapper-prod -s stsharedutilitiesprod --functions-version 3


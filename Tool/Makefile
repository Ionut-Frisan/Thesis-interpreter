run:
	dotnet build -c Release && \ 
	if [ "$(OS)" = "Windows_NT" ]; then \
		bin/Release/net9.0/Tool.exe ../MDA/ \
	else \
		bin/Release/net9.0/Tool ../MDA/ \
	fi \

clean:
	rm ../MDA/Expr.cs
	rm ../MDA/Stmt.cs
	
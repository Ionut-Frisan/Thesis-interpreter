run:
	dotnet build -c Release && \
	if [ "$(OS)" = "Windows_NT" ]; then \
		bin/Release/net9.0/Tool.exe ../MDA/Parsing/ ;\
	else \
		bin/Release/net9.0/Tool ../MDA/Parsing/ ;\
	fi

clean:
	rm ../MDA/Expr.cs
	rm ../MDA/Stmt.cs
	
run:
	pwd; \
	cd ../MDA; \
	pwd; \
	dotnet build -c Release && \
	find ../Examples -type f -name "*.mda" | while read file; do \
		echo "Running file: $$file"; \
		if [ "$(OS)" = "Windows_NT" ]; then \
			bin/Release/net9.0/MDA.exe "$$file"; \
		else \
			bin/Release/net9.0/MDA "$$file"; \
		fi \
		echo "--------------------------"; \
	done

benchmark:
	pwd; \
	cd ../MDA; \
	pwd; \
	dotnet build -c Release && \
	if [ "$(OS)" = "Windows_NT" ]; then \
		bin/Release/net9.0/MDA.exe ../Examples/benchmark.mda; \
	else \
		bin/Release/net9.0/MDA ../Examples/benchmark.mda; \
	fi
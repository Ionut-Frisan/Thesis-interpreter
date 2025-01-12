LOGPATH=../Examples/benchmark_results/
LOGFILE=$(LOGPATH)`date +'%y_%m_%d-%H:%M:%S'`.txt

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
	

benchmark_save:
	pwd; \
	cd ../MDA; \
	pwd; \
	dotnet build -c Release && \
	echo "Saving results to $(LOGFILE)"; \
	touch LOGFILE && \
	if [ "$(OS)" = "Windows_NT" ]; then \
		bin/Release/net9.0/MDA.exe ../Examples/benchmark.mda > $(LOGFILE); \
	else \
		bin/Release/net9.0/MDA ../Examples/benchmark.mda > $(LOGFILE); \
	fi
	

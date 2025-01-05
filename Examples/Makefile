run:
	pwd; \
	cd ../MDA; \
	pwd; \
	dotnet build -c Release && \
	find ../Examples -type f -name "*.mda" | while read file; do \
  		echo "Running file: $$file"; \
  		bin/Release/net9.0/MDA "$$file"; \
  		echo "--------------------------"; \
	done


benchmark:
	pwd; \
	cd ../MDA; \
	pwd; \
	dotnet build -c Release && \
	bin/Release/net9.0/MDA ../Examples/benchmark.mda
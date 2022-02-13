FROM bitnami/apache
LABEL maintainer "Julius Herzig <jh1521@ic.ac.uk>"
COPY WebGL-Dist/ /app/
USER 0
RUN chown -R 1001 /app
USER 1001

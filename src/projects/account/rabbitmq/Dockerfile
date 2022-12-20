FROM rabbitmq:3.11.4-management-alpine
COPY rabbitmq.conf /etc/rabbitmq
COPY definitions.json /etc/rabbitmq
RUN cat /etc/rabbitmq/rabbitmq.conf
ENV  RABBITMQ_SERVER_ADDITIONAL_ERL_ARGS "-rabbitmq_stream advertised_host localhost"
RUN rabbitmq-plugins enable rabbitmq_stream

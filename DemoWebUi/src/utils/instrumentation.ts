import { registerInstrumentations } from '@opentelemetry/instrumentation';
import {
  WebTracerProvider,
  BatchSpanProcessor, ConsoleSpanExporter, SimpleSpanProcessor,
} from '@opentelemetry/sdk-trace-web';
import { Resource } from '@opentelemetry/resources';
import { getWebAutoInstrumentations } from '@opentelemetry/auto-instrumentations-web';
import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-http';
import {
  SEMRESATTRS_SERVICE_NAME,
} from '@opentelemetry/semantic-conventions';
import {ZoneContextManager} from "@opentelemetry/context-zone";
import {W3CTraceContextPropagator} from "@opentelemetry/core";


// Define the service name for the resource
const resource = new Resource({
  [SEMRESATTRS_SERVICE_NAME]: 'demo-web-ui'
});

const exporter = new OTLPTraceExporter({
  url: 'http://localhost:4318/v1/traces',
});

const provider = new WebTracerProvider({
  resource: resource
});

// Uncomment for debugging purposes
//provider.addSpanProcessor(new SimpleSpanProcessor(new ConsoleSpanExporter()));

provider.addSpanProcessor(new BatchSpanProcessor(exporter));
provider.register({
  contextManager: new ZoneContextManager(),
  propagator: new W3CTraceContextPropagator(),
});

registerInstrumentations({
  instrumentations: [
    getWebAutoInstrumentations({
      '@opentelemetry/instrumentation-document-load': {},
      // load custom configuration for xml-http-request instrumentation
      '@opentelemetry/instrumentation-xml-http-request': {
        propagateTraceHeaderCorsUrls: [
          /.+/g,
        ],
      },
      // load custom configuration for fetch instrumentation
      '@opentelemetry/instrumentation-fetch': {
        propagateTraceHeaderCorsUrls: [
          /.+/g,
        ],
      },
    }),
  ],
});

export const instrumentation = provider.getTracer('http-client');

# EventHub

This is a set of helper tools to interact with an Event Hub. Assuming you have a working Event Hub, you can these tools to send events to it or to receive the events. There are three main tools:

- **EventHubSender**: simple console application to send number of events to a configured Event Hub
- **EventHubReceiver**: simple console application to receive events from a configured Event Hub (it uses *EventProcessorClient* with built-in support for checkpointing)
- **EventHubPartitionReceiver**: simple console application to receive events from a specific Event Hub partition (it uses basic *EventHubConsumerClient* with custom logic to persist checkpoints to a blob container)

## EventHubSender

This is a simple console application to send number of events to a configured Event Hub. 

Before you can run the application, you need to update `appsettings.json` configuration file and provide relevant Event Hub connection information.

Run with `--help` command line arguments to see the list of available options:

- `--number <number>`:  number of events to send in a single batch
- `--delay <delay>`: delay in seconds between batches of messages
- `--auto`: flag to indicate whether to send events repeatedly

**Example:** The following command would start the tool, send batch of 100 events to the configured Event Hub and stop. 

```shell
EventHubSender --number 100
```

**Example:** The following command would start the tool and continuously send batch of 5 events to the configured Event Hub. The tool will wait for 10 seconds between the batches.

```shell
EventHubSender --number 5 --auto --delay 10
```

## EventHubReceiver

This is a simple console application to receive events from a configured Event Hub. This particular tool uses *EventProcessorClient* with built-in support for checkpointing. 

Before you can run the application, you need to update `appsettings.json` configuration file and provide relevant Event Hub connection information and consumer group to be used to receive events. Besides that you also need to provide Blob storage connection information and blob container name, where checkpoints will be persisted. 

The application can be run in two modes, you can either provide a time for which the events should be received, or you can run it in a manual mode and the application will be receiving events until you press a key on your keyboard.

Run with `--help` command line arguments to see the list of available options:

- `--time <time>`:  time in seconds to listen for events and then stop.

**Example:** The following command would start the tool and receive events for 5 seconds and then stop.

```shell
EventHubReceiver --time 5
```

**Example:** The following command would start the tool and receive events until you press a key on your keyboard.

```shell
EventHubReceiver 
```

## EventHubPartitionReceiver

This is a simple console application to receive events from a configured Event Hub partition. This particular tool uses the basic *EventHubConsumerClient* to receive events. This base client doesn't have support for checkpoints, so the tool contains custom logic to persist checkpoints to a blob storage container.

Before you can run the application, you need to update `appsettings.json` configuration file and provide relevant Event Hub connection information and consumer group to be used to receive events. Besides that you also need to provide Blob storage connection information and blob container name, where checkpoints will be persisted. 

The application can be run in two modes, you can either provide a time for which the events should be received, or you can run it in a manual mode and the application will be receiving events until you press a key on your keyboard.

Run with `--help` command line arguments to see the list of available options:

- `--time <time>`:  time in seconds to listen for events and then stop.

**Example:** The following command would start the tool and receive events for 5 seconds and then stop.

```shell
EventHubPartitionReceiver --time 5
```

**Example:** The following command would start the tool and receive events until you press a key on your keyboard.

```shell
EventHubPartitionReceiver
```


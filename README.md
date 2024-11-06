IoTBridgeMqtt
================

Provides an IoT bridge between devices connected to [MQTT](https://mqtt.org/), for use in
closed intra-networks or enterprise networks, and the harmonized XMPP-based 
[Neuro-Foundation](https://neuro-foundation.io/) network, for open and secure cross-domain
interoperation on the Internet.

![Bridge topology](Graphs/Topology.svg)

To run the bridge, you need access to both an MQTT broker, and an XMPP broker, that supports
the Neuro-Foundation extensions. You can use the [Mosquitto broker](https://mosquitto.org/)
for MQTT, and the [TAG Neuron](https://lab.tagroot.io/Documentation/Neuron/InstallBroker.md)
for XMPP.

Running and configuring the bridge
-------------------------------------

The code is written using .NET Standard, and compiled to a .NET Core console application
that can be run on most operating systems. Basic configuration is performed using the
console interface during the first execution, and persisted. You can also provide the
corresponding configuration using environment variables, making it possible to run the
bridge as a container. If an environmental variable is missing, the user will be prompted
to input the value on the console.

| Environmental Variable    | Type    | Description                                                                                                           |
|:--------------------------|:--------|:----------------------------------------------------------------------------------------------------------------------|
| `XMPP_HOST`               | String  | XMPP Host name                                                                                                        |
| `XMPP_PORT`				| Integer | Port number to use when connecting to XMPP (default is `5222`)                                                        |
| `XMPP_USERNAME`			| String  | User name to use when connecting to XMPP.                                                                             |
| `XMPP_PASSWORD`			| String  | Password (or hashed password) to use when connecting to XMPP. Empty string means a random password will be generated. |
| `XMPP_PASSWORDHASHMETHOD`	| String  | Algorithm or method used for password. Empty string means the password is provided in the clear.                      |
| `XMPP_APIKEY`				| String  | API Key. If provided together with secret, allows the application to create a new account.                            |
| `XMPP_APISECRET`			| String  | API Secret. If provided together with key, allows the application to create a new account.                            |
| `MQTT_HOST`               | String  | MQTT Host name														                                                  |
| `MQTT_TLS`                | String  | If TLS encryption is to be used when connecting to the MQTT broker.	                                                  |
| `MQTT_PORT`               | String  | Port number to use when connecting to MQTT (default is `1883` for unencrypted MQTT and 8883 for encrypted MQTT).      |
| `MQTT_USERNAME`           | String  | User name to use when connecting to MQTT. Can be empty if no user credentials are provided.                           |
| `MQTT_PASSWORD`           | String  | Password to use when connecting to XMPP. Can be empty if no user credentials are provided.                            |
| `REGISTRY_COUNTRY`		| String  | Country where the bridge is installed.                                                                                |
| `REGISTRY_REGION`			| String  | Region where the bridge is installed.                                                                                 |
| `REGISTRY_CITY`			| String  | City where the bridge is installed.                                                                                   |
| `REGISTRY_AREA`			| String  | Area where the bridge is installed.                                                                                   |
| `REGISTRY_SRTEET`			| String  | Street where the bridge is installed.                                                                                 |
| `REGISTRY_STREETNR`		| String  | Street number where the bridge is installed.                                                                          |
| `REGISTRY_BUILDING`		| String  | Building where the bridge is installed.                                                                               |
| `REGISTRY_APARTMENT`		| String  | Apartment where the bridge is installed.                                                                              |
| `REGISTRY_ROOM`			| String  | Room where the bridge is installed.                                                                                   |
| `REGISTRY_NAME`			| String  | Name associated with bridge.                                                                                          |
| `REGISTRY_LOCATION`		| Boolean | If location has been completed. (This means, any location-specific environment variables not provided, will be interpreted as intensionally left blank, and user will not be prompted to input values for them. |

Claiming ownership of bridge
-------------------------------

Once the bridge has been configured, it will generate an `iotdisco` URI, and save it to its
programd data folder. It will also create a file with extension `.url`, containing a shortcut
with the `iotdisco` URI inside. A `.png` file with a QR code will also be generated. All three
files contain information about the bridge, and allows the owner to claim ownership of it.
This can be done by using the [Neuro-Access App](https://github.com/Trust-Anchor-Group/NeuroAccessMaui).
This app is also downloadable for [Android](https://play.google.com/store/apps/details?id=com.tag.NeuroAccess) 
and [iOS](https://apps.apple.com/app/neuro-access/id6446863270). You scan the QR code (or
enter it manually), and claim the device. Once the device is claimed by you, you will receive
notifications when someone wants to access the deice. They will only be able to access it
with the owner's permission. For more information, see:

* [Registration, discovery & ownership process](https://neuro-foundation.io/Discovery.md)
* [Decision support for deviceos](https://neuro-foundation.io/DecisionSupport.md)
* [Provisioning for owners](https://neuro-foundation.io/Provisioning.md)


Configuring the bridge
-------------------------

The bridge can be configured in detail by a client that implements the [concentrator interface](https://neuro-foundation.io/Concentrator.md).
Concentrators consist of *data sources*, each containing tree structures of *nodes*. Nodes may be partitioned into
*partitions*, which permits the nesting of subsystems seamlessly into container systems. Each node can be
of different types, and have different properties and underlying functionality. They can each implement then
[sensor interface](https://neuro-foundation.io/SensorData.md) and [actuator interface](https://neuro-foundation.io/ControlParameters.md).

You can use the [Simple IoT Client](https://waher.se/IoTGateway/SimpleIoTClient.md) to configure concentrators and their nodes in detail.
An initial setup is done using the initial configuration of the bridge. The client is also available in the
[IoTGateway](https://github.com/Neuro-Foundation/IoTGateway) repository, in the
[Clients folder](https://github.com/Neuro-Foundation/IoTGateway/tree/master/Clients/Waher.Client.WPF).

Node Types
-------------

The bridge includes several different node types that can be used to configure its operation:

*	The `MQTT Broker` maintains a connection to an MQTT Broker, and allows the bridge to subcribe to
	content published on topics, as well as publish content to topics on the broker. The topic
	hierarchy will be modelled using `MQTT Topic` nodes, or derivatives. Common data types are
	recognized and parsed. You can read each topic individually, or a parent topic, and receive
	information from all child topics, as field values.

*	The `XMPP Broker` maintains a connection to an XMPP Broker. It allows the bridge to connect
	to other entities on the federated network and communicate with them. It supports communication
	with remote standalone sensors and actuators, as well as remote concentrators embedding devices
	into data sources and nodes. Such concentrators can be bridges to other protocols and networks.
	
	**Note**: The bridge has a client-to-server connection by default, setup during initial
	configuration. Through this connection, the bridge acts as a concentrator. Through the use of
	`XMPP Broker` nodes you can setup additional XMPP connections to other brokers. In these cases
	the bridge will only act as a client, to connect to remove devices for the purposes of interacting
	with them.

*	`IEEE 1451` nodes are derivatives of MQTT nodes, and implement support for the `IEEE 1451` family
	of standards, of which `IEEE 1451.1.6` manages communication over MQTT.

*	`IP Host` nodes allow you to monitor network hosts accessible from the bridge.

*	`Script` nodes allow you to create nodes with custom script logic. They can be used to interface
	bespoke devices in the network accessible from the bridge, for example.

*	`Virtual` nodes are placeholders where external logic (or script logic) can aggregate information
	in a way that makes them accessible by others in the federated network.
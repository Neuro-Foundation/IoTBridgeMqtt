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

The code is written using .NET Standard, and compiled to a .NET Core console application
that can be run on most operating systems. Basic configuration is performed using the
console interface during the first execution, and persisted. You can also provide the
corresponding configuration using environment variables, making it possible to run the
bridge as a container.
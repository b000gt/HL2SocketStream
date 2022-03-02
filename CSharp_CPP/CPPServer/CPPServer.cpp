// CPPServer.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "iostream"
#include "winsock2.h"
#include <sstream>
#include <chrono>
#include <thread>

std::string test_reply = "test reply";

const int BUFFER_LENGTH = 1024;

class UdpStreamingServer {
public:
	UdpStreamingServer(int clientPort) {
		this->Port = clientPort;
		this->SenderAddrSize = sizeof(this->SenderAddr);
		WSAStartup(MAKEWORD(2, 2), &this->wsaData);
		this->RecvSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
		this->RecvAddr.sin_family = AF_INET;
		this->RecvAddr.sin_port = htons(Port);
		this->RecvAddr.sin_addr.s_addr = htonl(INADDR_ANY);
		bind(this->RecvSocket, (SOCKADDR*)&this->RecvAddr, sizeof(this->RecvAddr));
	}
	~UdpStreamingServer() {
		closesocket(RecvSocket);
		WSACleanup();
	}
	char* receive() {
		char RecvBuf[BUFFER_LENGTH];
		recvfrom(this->RecvSocket, RecvBuf, BUFFER_LENGTH, 0, (SOCKADDR*)&this->SenderAddr, &this->SenderAddrSize);
		return RecvBuf;
	}

	void send(const char* message, size_t messageSize) {
		sendto(RecvSocket, message, messageSize, 0, (SOCKADDR*)&this->SenderAddr, this->SenderAddrSize);
	}
	void send(std::string message) {
		this->send(message.data(), message.size());
	}
private:
	WSADATA wsaData;
	SOCKET RecvSocket;
	sockaddr_in RecvAddr;
	int Port;
	sockaddr_in SenderAddr;
	int SenderAddrSize;
};


int main()
{
	UdpStreamingServer server(28250);
	time_t last_kl_received = 0;
	for (int i = 0; i < 200000; i++) {
		if (std::time(0) - last_kl_received > 2) {
			std::cout << "Waiting for Client...\n";
			std::string kl_signal;
			while (!kl_signal.starts_with("KEEPALIVE")) {
				kl_signal = server.receive();
			}
			last_kl_received = std::time(0);
		}
		std::cout << "Sending: " << i << "\n";
		server.send(std::to_string(i));
	}
}
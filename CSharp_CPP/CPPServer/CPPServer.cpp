// CPPServer.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "iostream"
#include "winsock2.h"
#include <sstream>
#include <chrono>
#include <thread>

std::string test_reply = "test reply";

const int BUFFER_LENGTH = 512;

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
		this->ThreadIsAlive = true;
		//TODO start keepalive thread
	}
	~UdpStreamingServer() {
		//TODO stop keepalive thread
		closesocket(RecvSocket);
		WSACleanup();
	}

	void revieveKeepAlive() {
		while (this->ThreadIsAlive) {
			receive();
		}
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
	std::thread KeepAliveThread;
	bool ThreadIsAlive;
};


int main()
{
	float matrix[4][4]{
		{0, 0, 0, 0.00001},
		{0, 0, 0, 0.00001},
		{0, 0, 0, -0.001},
		{0, 0, 0, 1} };
	std::stringstream matrix_string;
	for (int i = 0; i < 4; i++)
	{
		for (int j = 0; j < 4; j++) {
			matrix_string << matrix[i][j] << "\t";
		}
		matrix_string << "\n";
	}
	UdpStreamingServer server(28250);
	time_t last_kl_received = 0;
	std::cout << "Waiting for Client...\n";
	std::string kl_signal;
	while (!kl_signal.starts_with("KEEPALIVE")) {
		kl_signal = server.receive();
	}
	for (int i = 0; i < 200000; i++) {
		/*if (std::time(0) - last_kl_received > 2) {
			std::cout << "Waiting for Client...\n";
			std::string kl_signal;
			while (!kl_signal.starts_with("KEEPALIVE")) {
				kl_signal = server.receive();
			}
			last_kl_received = std::time(0);
		}*/
		std::this_thread::sleep_for(std::chrono::milliseconds(10));
		std::cout << "Sending:\n" << matrix_string.str() << "\n";
		server.send(matrix_string.str());
	}
}
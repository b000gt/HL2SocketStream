// CPPSender.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "iostream"
#include <Winsock2.h>
#include <Ws2tcpip.h>
#include <string>
#include <chrono>
#include <thread>
#include <iostream>
#include <charconv>

using namespace std;

void main()

{

	WSADATA wsaData;

	SOCKET SendSocket;

	const int MAX_DIGITS = 10;

	sockaddr_in RecvAddr;

	int Port = 8080;

	int BufLen = 64;

	const char* IP_ADDRESS_S = "127.0.0.1";

	WSAStartup(MAKEWORD(2, 2), &wsaData);

	SendSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_TCP);

	RecvAddr.sin_family = AF_INET;

	RecvAddr.sin_port = htons(Port);

	cout << inet_pton(AF_INET, IP_ADDRESS_S, &RecvAddr.sin_addr);
	const char* SendBuf;

	system("pause");
	for (int i = 0; i < 200000; i++) {
		this_thread::sleep_for(chrono::milliseconds(5));
		cout << "Sending: " << to_string(i).c_str() << " to reciever.\n";
		SendBuf = to_string(i).c_str();
		BufLen = sizeof(SendBuf);
		sendto(SendSocket, SendBuf, BufLen, 0, (SOCKADDR*)&RecvAddr, sizeof(RecvAddr));
	}

	cout << "Finished sending. Closing socket.";

	closesocket(SendSocket);

	cout << "Exiting.";

	WSACleanup();
	system("pause");
}

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file

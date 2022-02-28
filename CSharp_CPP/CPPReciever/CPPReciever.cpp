// CPPReciever.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "iostream"

#include "winsock2.h"

using namespace std;

void main()

{

	WSADATA wsaData;

	SOCKET RecvSocket;

	sockaddr_in RecvAddr;

	int Port = 28250;

	char RecvBuf[1024];

	int BufLen = 1024;

	sockaddr_in SenderAddr;

	int SenderAddrSize = sizeof(SenderAddr);

	WSAStartup(MAKEWORD(2, 2), &wsaData);

	RecvSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);

	RecvAddr.sin_family = AF_INET;

	RecvAddr.sin_port = htons(Port);

	RecvAddr.sin_addr.s_addr = htonl(INADDR_ANY);

	bind(RecvSocket, (SOCKADDR*)&RecvAddr, sizeof(RecvAddr));

	cout << "Receiving datagrams...";

	recvfrom(RecvSocket, RecvBuf, BufLen, 0, (SOCKADDR*)&SenderAddr, &SenderAddrSize);

	cout << "Finished receiving. Closing socket.";
	cout << RecvBuf;

	closesocket(RecvSocket);

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

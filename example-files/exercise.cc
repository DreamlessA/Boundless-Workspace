// Copyright [2018] <XXXXX>


#include <fcntl.h>
#include <errno.h>
#include <unistd.h>
#include <cstdio>
#include <cstring>
#include <iostream>

#include "./Util.h"

#define BUF 256

void Usage(char *progname);

int main(int argc, char *argv[]) {
  // check if the number of command line argument is correct
  if (argc != 4) {
    Usage(argv[0]);
  }

  // try to open the local file
  int file_fd = open(argv[3], O_RDONLY);
  if (file_fd == -1) {
    Usage(argv[0]);
  }

  // try to read the port number from command line argument
  unsigned short port = 0;
  if (sscanf(argv[2], "%hu", &port) != 1) {
    Usage(argv[0]);
  }

  // get an appropriate sockaddr structure
  struct sockaddr_storage addr;
  size_t addrlen;
  if (!LookupName(argv[1], port, &addr, &addrlen)) {
    Usage(argv[0]);
  }

  // connect to the remote host
  int socket_fd;
  if (!Connect(addr, addrlen, &socket_fd)) {
    Usage(argv[0]);
  }

  // try to read the bytes from the local file
  // then write those bytes over the TCP connection
  char readbuf[BUF];
  int read_res, write_res;
  while (1) {
    read_res = Read(file_fd, readbuf, BUF);
    if (read_res == 0) {
      break;
    } else if (read_res < 0) {
      std::cerr << "file read failure: " << strerror(errno) << std::endl;
      close(file_fd);
      close(socket_fd);
      return EXIT_FAILURE;
    }

    write_res = Write(socket_fd, readbuf, read_res);
    if (write_res != read_res) {
      std::cerr << "socket write failure: " << strerror(errno) << std::endl;
      close(file_fd);
      close(socket_fd);
      return EXIT_FAILURE;
    }
  }

  // clean up
  close(file_fd);
  close(socket_fd);
  return EXIT_SUCCESS;
}

void Usage(char *progname) {
  std::cerr << "usage: " << progname << " hostname port filename" << std::endl;
  exit(EXIT_FAILURE);
}

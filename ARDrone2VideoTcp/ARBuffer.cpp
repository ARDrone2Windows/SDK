#include "pch.h"
#include "ARBuffer.h"


ARBuffer::ARBuffer(unsigned char* dataToCopy, uint32_t size)
	: _data(new unsigned char[size]), _size(size)
{
	memcpy_s(_data.get(), size, dataToCopy, size);
}

ARBuffer::ARBuffer( uint32_t size )
	: _data(new unsigned char[size]), _size(size)
{

}


ARBuffer::~ARBuffer(void)
{
}

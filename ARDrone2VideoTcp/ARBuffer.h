#pragma once
#include <memory>
#include <stdint.h>

class ARBuffer 
{
private:
	std::unique_ptr<unsigned char[]> _data;
	uint32_t _size;
	// non copyable
	ARBuffer(const ARBuffer& other){}

public:
	uint32_t getSize()const{
		return _size;
	}
	unsigned char* getData() const{
		return _data.get();
	}
	ARBuffer(unsigned char* dataToCopy, uint32_t size);
	ARBuffer(uint32_t size);
	ARBuffer(ARBuffer&& moved):_data(std::move(moved._data)), _size (moved._size){
		moved._size = 0;
	}
	~ARBuffer(void);
};


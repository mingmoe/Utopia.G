# CMakeList.txt : CMake project for Utopia.FastNoise, include source and define
# project specific logic here.
#
cmake_minimum_required (VERSION 3.27)

include("${CMAKE_CURRENT_LIST_DIR}/../share.cmake")

project("Utopia.FastNoise" VERSION ${READ_UTOPIA_VERSION} LANGUAGES CXX C)

set(FASTNOISE2_NOISETOOL OFF)
set(FASTNOISE2_TESTS OFF)
set(BUILD_SHARED_LIBS ON)

add_subdirectory("${CMAKE_CURRENT_LIST_DIR}/fastnoise")
# include("${CMAKE_CURRENT_LIST_DIR}/fastnoise/CMakeLists.txt")


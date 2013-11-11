function sub = block(blocks, block_sizes)
% BLOCK Return a vector of subscripts corresponding to the specified blocks.
% sub = block(blocks, block_sizes)
% 
% e.g., block([2 5], [2 1 2 1 2]) = [3 7 8].

blocks = blocks(:)';
block_sizes = block_sizes(:)';
skip = [0 cumsum(block_sizes)];
start = skip(blocks)+1;
fin = start + block_sizes(blocks) - 1;
sub = [];
for j=1:length(blocks)
  sub = [sub start(j):fin(j)];
end


% block_sizes lists the size of a bunch of blocks
% that is, if you have some array somewhere, then
% [2 1 2 1 2] would mean that the input is 8 long
% and divided into first a block of 2, then 1, then 2, then 1, then 2
%
% blocks gives the number of block you want,
% so [2 5] corresponds to the 2nd and 5th blocks,
% which are a block of 1 and a block of 2 respectively
%
% this function returns an array of indicies for the array
% that has been 'blockified' so that those indicies correspond
% to all of the blocks that you wanted from the array
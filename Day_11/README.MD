# --- Day 11: Plutonian Pebbles ---
Part 1, easy peasy less than 5 minutes with a regular List and a loop.  
Part 2... well that was a ride. I first tried to optimize my function code, underestimating the impact of the Insert.  
Then thought about a LinkedList, to reduce memory moves. Then thought again about what actually needed to be done here.  
And then i realised, i dont need the order, but rather just the numbers, so back to a regular list, this time with add instead of insert.  
This also was futile, since i ran out of memory and my code was too slow.  
So, i attached the debugger, waited a bit and checked the data:  
There were a lot of duplicate numbers.  
And then it struck me. The easiest solution for this is to make use of a dictionary.
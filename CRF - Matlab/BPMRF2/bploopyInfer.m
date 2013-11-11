function [bel, belE, logZ, msg, niter] = bploopyInfer(engine, pot, localEv)
% BP_MRF2_GENERAL Belief propagation on an MRF with pairwise potentials
%
% Input:
% E(i,j) = edge number or 0 if absent
% pot{e}(ki,kj) = potential on edge between nodes i,j
% localEv{i}(k) = Pr(observation at node i | Xi=k)
%
%
% Output:
% bel{i}(k) = P(Xi=k|evidence)
% belE{e}(:) = P(edge e)
% neglogZ = Bethe approximation to -log(Z) 
% niter contains the number of iterations used 
%
% [ ... ] = bp_mrf2(..., 'param1',val1, 'param2',val2, ...)
% allows you to specify optional parameters as name/value pairs.
% Parameters names are below [default value in brackets]
%
% max_iter - max. num. iterations [ 5*nnodes]
% momentum - weight assigned to old message in convex combination
%            (useful for damping oscillations) - currently ignored i[0]
% tol      - tolerance used to assess convergence [1e-3]
% maximize - 1 means use max-product, 0 means use sum-product [0]
% verbose - 1 means print error at every iteration [0]
%
% fn - name of function to call at end of every iteration [ [] ]
% fnargs - we call feval(fn, bel, iter, fnargs{:}) [ [] ]

engine = engine;
E = engine.E;
nstates = engine.nstates;
verbose = engine.verbose;
maximize = engine.maximize;

nnodes = length(E);
nedges = length(find(E))/2;
E = E;
adj_mat = (E>0);

% initialise messages
prod_of_msgs = cell(1, nnodes);
old_bel = cell(1, nnodes);
old_msg = cell(1, nedges);
for i=1:nnodes
  prod_of_msgs{i} = localEv{i};  % initialize prod_of_mesgs w/ the local evidence for a node
                                 % where localEv is a vector of site
                                 % potentials, evaluated for each label
  old_bel{i} = localEv{i};  % initialize old_bel similarly
  nbrs = find(adj_mat(:,i));   %find all of i's neighbors.  I'm not sure why it
                               % is adj_mat(:,i) instead of adj_mat(i,:)
  for j=nbrs(:)'
    %fprintf('i=%d,j=%d,e=%d,s=%d\n', i, j, E(i,j), nstates(j));
    old_msg{E(i,j)} = normalise(ones(nstates(j),1));
       % normalise(ones(nstates(j),1)) should give a vector of height j in
       % which all of the entries are equal and sum to one
       %
       % this makes the E(i,j)th old message equal to that vector
  end
end


converged = 0;
iter = 1;
done = 0;

while ~converged & (iter <= engine.max_iter) %stop when appropriate
  
  % each node sends a msg to each of its neighbors
  for i=1:nnodes
    nbrs = find(adj_mat(i,:));  %same as above, but switched (:,i) -> (i,:)
    for j=nbrs(:)'
      if i<j     % this if guarantees that your potential is from the upper right
                 % triangle of the interaction potential matrix
        pot_ij = pot{E(i,j)};
      else
        pot_ij = pot{E(j,i)}';
      end
      pot_ij = pot_ij';
      % now pot_ij(xj, xi)  so pot_ij * msg(xi) = sum_xi pot(xj,xi) msg(xi) = f(xj)

      if 1  % I don't understand why there are two blocks of code like this (if 1 & if ~done)
        % Compute temp = product of all incoming msgs except from j
        % by dividing out old msg from j from the product of all msgs sent to i
        temp = prod_of_msgs{i};  % was initialized to local evidence vector...
        m = old_msg{E(j,i)}; 
        if any(m==0)
          fprintf('m=0, iter=%d, send from i=%d to j=%d\n', iter, i, j);
          done=1;
          break;  % should break out of neighbors loop... I think.  Makes the 
                  % m + (m==0) thing kinda weird and seemingly superfluous
          %keyboard
        end
        m = m + (m==0); % valid since m(k)=0 => temp(k)=0, so can replace 0's with anything
        %fprintf('the current value of m is %d', m);
        temp = temp ./ m;
        temp_div = temp;
      end
      
      if ~done
        % Compute temp = product of all incoming msgs except from j in obvious way
    	%temp = ones(nstates(i),1);
    	temp = localEv{i};
        for k=nbrs(:)'
    	  if k==j, continue, end;  % this is a one-liner if statement
          temp = temp .* old_msg{E(k,i)};
        end
        %assert(approxeq(temp, temp_div))
    	assert(approxeq(normalise(pot_ij * temp), normalise(pot_ij * temp_div)))
      end
      
      if maximize
        newm = max_mult(pot_ij, temp); % bottleneck
      else
        newm = pot_ij * temp;  % pot_ij is matrix, temp is vector
      end
      newm = normalise(newm);
      new_msg{E(i,j)} = newm;  % i think this is the new think i sends to j
    end % for j 
  end % for i
  old_prod_of_msgs = prod_of_msgs;
  
  % each node multiplies all its incoming msgs and computes its local belief
  for i=1:nnodes
    nbrs = find(adj_mat(:,i));
    prod_of_msgs{i} = localEv{i};  % this is what prod_of_msgs is initialized
                                   % to from the start, seems kinda a weird
                                   % thing, unless localEv is modified
                                   % somewhere
    for j=nbrs(:)'
      prod_of_msgs{i} = prod_of_msgs{i} .* new_msg{E(j,i)};
      % ok, the (j,i) indicates the direction of the message, and this bit
      % takes each message from a neighbor, and brings it into the product
      % for this node
    end
    new_bel{i} = normalise(prod_of_msgs{i});  %normalise makes this a probability
  end
  
  %I AM HERE I AM HERE I AM HERE!!!
  err = abs(cat(1,new_bel{:}) - cat(1, old_bel{:}));

  converged = all(err < engine.tol) | done;  %WHY IS | done HERE?
  if verbose, fprintf('error at iter %d = %f\n', iter, sum(err)); end
  %celldisp(new_bel)
  if ~isempty(engine.fn) %does this actually do anything?
    if isempty(engine.fnargs)
      feval(engine.fn, new_bel);
    else
      feval(engine.fn, new_bel, iter, engine.fnargs{:});
    end
  end
  
  iter = iter + 1;
  old_msg = new_msg;
  old_bel = new_bel;
end % while

niter = iter-1;

if verbose, fprintf('converged in %d iterations\n', niter); end

bel = new_bel;
msg = new_msg;

if nargout >= 2,
  belE = computeEdgeBel(E, pot, bel, msg);
end

if nargout >= 3,
  neglogZ = betheMRF2(E, pot, localEv, bel, belE);
  logZ = -neglogZ;
end

function [g, gdata, gprior] = crfgrad(net, x, t)
% crfgrad CRF gradient function = gradient of negative complete-data log-likelihood
% function [g, gdata, gprior, gs] = crfgrad(net, x, t)
%
% x{s,i}(:) - cell for node i case s is a column feature vector
% t(s,i) - each ROW is the set of desired discrete output labels (targets)
%
% This function can be used by netlab's optimization  routines
% This requires inference so is slow!

% Inference means computing the marginals and logZ.
% logZ (which is slow to compute) is only needed for crferr, not crfgrad.
% The code is explained in crfErrAndGrad.


[ncases nnodes] = size(t);
ns = net.nstates;  % number of states per node.  This is a vector and each node
                   % can, in theory, have a different number of states

%it seems likely that this will store the gradient.  To get the gradient we
%must calculated each component.  This means that each parameter gets a
%separate entry.  I think that is why gs has net.nparamsAdjustable columns.
%
%We must also find the gradient for every one of the test cases given to us
%for training.  That is why there are ncases (# test cases) rows.  Thus
%each row in gs is the gradient for a given test case
gs = zeros(ncases, net.nparamsAdjustable);
infName = sprintf('%sInfer', net.infEngineName);
for s=1:ncases
  % inference happens here in order to calculate the expected
  % value of P(y|x,theta) based on the current model distribution
  % (the current values of the parameters)
  [localEv, logLocalEv] = crfMkLocalEv(net, x(s,:)); 
     %local evidence seems to be the evaluaction of the site potential
     %functions.  i.e., e^(weigths * feature functions).  This will
     %return a vector for each node, where the entries in the vector
     %correspond to the label being given to the node
  pot = crfMkPot(net);
    % potentials should probably be normalized down columns or across
    % rows.  Just pick one and be consistent
  [bel, belE] = feval(infName, net.infEngine, pot, localEv);
  % Still not sure exactly what belief is, or how it could be on
  % nodes and edges

  % bel keeps track of the probability of a given state!
  % bet(q,t) is probability node t has label q!
  
  bsi = net.nparamsPerNodeEclass; %size of parameter array for nodes.  
                                  %Tells us how a vector of all the
                                  %parameters would be broken into blocks
                                  %by which node they are parameters for
                                    
  gI = zeros(sum(bsi),1);  % stores the gradient for site parameters
  adjustableEdges = find(net.adjustableEdgeEclassBitv); %?????????
  bse = net.nstatesPerEdgeEclass(adjustableEdges); % similary to bsi, but for edges
  gE = zeros(sum(bse),1); %stores the gradient for the interaction params

  for i=1:nnodes
    ec = net.eclassNode(i); % Gets index denoting a particular node.  I think it is just i...
    if length(net.w{ec})>0  % If this node has parameters
      trueBel = zeros(ns(i),1); % Make a vector for this training case
      trueBel(t(s,i)) = 1;  % Tell which label is correct for this training case
      if net.clampWeightsForOneState %??????????????
    	trueBel = trueBel(2:end);
    	bel{i} = bel{i}(2:end);
      end
        %the x{s,i} is the evaluated site potential functions, for node
        %i and training set s
      clampedFvec = computeExpectedFvec(x{s,i}, trueBel);
      expectedFvec = computeExpectedFvec(x{s,i}, bel{i});
      % this looks to be the gradient w.r.t. the parameters
      % for single nodes.  It does indeed look to be the difference between
      % the real and expected sum over features
      gI(block(ec,bsi)) = gI(block(ec,bsi)) + (clampedFvec - expectedFvec); 
        %block(ec,bsi) means give the indicies of the block corresponding
        %to ec (the node number), if the array in question has been
        %broken into blocks according to bsi.  I think this makes it so
        %that the gradient is only updated for the parameters (node?) in question, or
        %some such
    end
    for j=i+1:nnodes  % for the remaining node pairs
      e = net.E(i,j);   % net.E holds the 'edgeNumbers' that are essentially and
                        % index for the edges
      if e>0 % really is e != 0, which means there is an edge between i & j
       	ec = net.eclassEdge(e);  %Gets 'index' of a particular edge
        ec2 = find_equiv_posns(ec, adjustableEdges); %this finds the positions where
                                                     %ec appears in
                                                     %adjustable edges.
                                                     %I'm not sure exactly
                                                     %why
        if ~isempty(ec2)
             % the next two lines make a true belief matrix similar to the
             % true belief vector from above.  This is constructed from the
             % training data, which is how we know a given belief is true
             % and all the rest are false
             trueBel = zeros(ns(i), ns(j));
             trueBel(t(s,i), t(s,j)) = 1;
             
             % this is the 'ideal' feature vector which perfectly reflects
             % reality.  trueBel(:) turns the matrix into a column vector
             % by having the first column in the old matrix at the top of
             % the column vector, then the second column immediately below
             % the first, and so on.
             clampedFvec = trueBel(:);
             
             % this is the belief predicted by the CRF doing inference
             % using its current parameters.  Also put into column vector
             % form
             expectedFvec = belE{e}(:);
             
             % this gives the gradient of the edges, but only for this 
             % paremter set [edge?] (that is what the blockin does, i think)
             gE(block(ec2,bse)) = gE(block(ec2,bse)) + (clampedFvec - expectedFvec);
        end
      end
    end
  end
  
  % the way that gI and gE are put together in this matrix makes it look
  % like they must have the same length...
  gs(s,:) = -[gI; gE]'; % minimize NEG log lik
  %fprintf('crfGrad, s=%d\n', s); gs(s,:)
  assert(~any(isnan(gs(s,:))))
end
gdata = sum(gs,1);
[g, gdata, gprior] = gbayes(net, gdata);
drawnow % allow for keyboard interrupt

if 1
%fprintf('crfgrad\n');
gdata;
net.pot{1};
net.w{1};
end

module {:extern "Bison.Razor.Models"} Models {
    class {:extern} Taxon {
        function {:extern} IsSubTaxon(ancestor: Taxon): bool
    }

    class {:extern} Observation {
        function {:extern} getTaxon(): Taxon
    }
}

module FilterByTaxon {
    import opened Models

    function FilterBy(root: Taxon, obs: seq<Observation>): seq<Observation>
        ensures forall o: Observation :: o in FilterBy(root, obs) ==> o in obs && o.getTaxon().IsSubTaxon(root)
        ensures forall o: Observation :: o in obs && o.getTaxon().IsSubTaxon(root) ==> o in FilterBy(root, obs)
        ensures |FilterBy(root, obs)| <= |obs|
        decreases |obs|
    {
        if |obs| == 0 then []
        else
        assert obs == [obs[0]] + obs[1..];
        if obs[0].getTaxon().IsSubTaxon(root)
        then [obs[0]] + FilterBy(root, obs[1..])
        else FilterBy(root, obs[1..])
    }
}

module FilterWrapper {
    import opened FilterByTaxon
    import opened Models

    // Wrapper called from C#. Returns a plain C# array so the caller
    // does not have to deal with Dafny sequences on the way out.
    method FilterObservations(root: Taxon, obs: seq<Observation>) returns (result: array<Observation>)
        ensures fresh(result)
        ensures result[..] == FilterBy(root, obs)
        // Soundness: everything returned comes from the input and belongs to root (or a subtaxon)
        ensures forall o: Observation :: o in result[..] ==> o in obs && o.getTaxon().IsSubTaxon(root)
        // Completeness: every input observation belonging to root (or a subtaxon) is returned
        ensures forall o: Observation :: o in obs && o.getTaxon().IsSubTaxon(root) ==> o in result[..]
    {
        var filtered := FilterBy(root, obs);
        result := new Observation[|filtered|](i requires 0 <= i < |filtered| => filtered[i]);
        assert result[..] == filtered;
    }
}